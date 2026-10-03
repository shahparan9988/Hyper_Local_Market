using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Infrastructure.Migrations
{
    // Called once, at the end of the scaffolded AddSellerProductsEditor.Up method.
    // EF generates the schema and snapshot. This helper only adapts existing records.
    public static class SellerProductsDataBackfill
    {
        public static void Apply(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
            DO $hlm$
            BEGIN
                IF EXISTS (
                    SELECT 1 FROM "Products" p JOIN "Stores" s ON s."Id" = p."StoreId"
                    WHERE s."CountryCode" NOT IN ('BD', 'AU')
                ) THEN
                    RAISE EXCEPTION 'Seller products support BDT/BD and AUD/AU. Review products in stores with another country before migrating.';
                END IF;
                IF EXISTS (
                    SELECT 1 FROM "ProductVariants" v
                    JOIN "Products" p ON p."Id" = v."ProductId"
                    JOIN "Stores" s ON s."Id" = p."StoreId"
                    WHERE trim(v."PriceCurrencyCode") <> CASE WHEN s."CountryCode" = 'AU' THEN 'AUD' ELSE 'BDT' END
                       OR (v."CompareAtPriceCurrencyCode" IS NOT NULL AND trim(v."CompareAtPriceCurrencyCode") <> trim(v."PriceCurrencyCode"))
                ) THEN
                    RAISE EXCEPTION 'Existing variant currencies do not match their store. Review them explicitly; this migration does not convert money.';
                END IF;
            END $hlm$;

            UPDATE "Products" p SET
                "CurrencyCode" = CASE WHEN s."CountryCode" = 'AU' THEN 'AUD' ELSE 'BDT' END,
                "IsPickupAvailable" = s."IsPickupAvailable"
            FROM "Stores" s WHERE s."Id" = p."StoreId";

            -- Keep inactive historical variants, but do not reactivate them in the editor.
            UPDATE "ProductVariants" SET "IsListed" = ("Status" = 'Active');

            WITH ordered AS (
                SELECT "Id", (row_number() OVER (
                    PARTITION BY "ProductId"
                    ORDER BY "IsListed" DESC, "IsDefault" DESC, "CreatedAtUtc", "Id"
                ) - 1)::integer AS position
                FROM "ProductVariants"
            )
            UPDATE "ProductVariants" v SET "CatalogOrder" = o.position
            FROM ordered o WHERE o."Id" = v."Id";

            -- Clear first to respect the existing unique partial default-variant index.
            UPDATE "ProductVariants" SET "IsDefault" = FALSE;
            UPDATE "ProductVariants" SET "IsDefault" = TRUE
            WHERE "IsListed" AND "CatalogOrder" = 0;

            -- A formerly incomplete published record must not become a free listing.
            UPDATE "Products" p SET "Status" = 'Draft'
            WHERE p."Status" = 'Active' AND NOT EXISTS (
                SELECT 1 FROM "ProductVariants" v WHERE v."ProductId" = p."Id" AND v."IsListed"
            );

            INSERT INTO "ProductVariants" (
                "Id", "ProductId", "DisplayName", "SalesUnit", "MinimumOrderQuantity",
                "QuantityIncrement", "IsDefault", "Status", "CreatedAtUtc", "UpdatedAtUtc",
                "PriceAmount", "PriceCurrencyCode", "IsPriceSet", "IsListed", "CatalogOrder"
            )
            SELECT gen_random_uuid(), p."Id", 'Standard', 'Each', 1, 1, TRUE, 'Active',
                p."CreatedAtUtc", p."UpdatedAtUtc", 0, p."CurrencyCode", FALSE, TRUE, 0
            FROM "Products" p WHERE NOT EXISTS (
                SELECT 1 FROM "ProductVariants" v WHERE v."ProductId" = p."Id" AND v."IsListed"
            );

            UPDATE "Products" p SET "VariantOptionName" = COALESCE(
                NULLIF((SELECT left(string_agg(o."Name", ' / ' ORDER BY o."DisplayOrder", o."Id"), 50)
                    FROM "ProductOptions" o WHERE o."ProductId" = p."Id"), ''), 'Options')
            WHERE (SELECT count(*) FROM "ProductVariants" v WHERE v."ProductId" = p."Id" AND v."IsListed") > 1;
            """);
        }
    }

}
