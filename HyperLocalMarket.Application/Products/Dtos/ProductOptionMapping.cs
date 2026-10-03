using HyperLocalMarket.Domain.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Products.Dtos
{
    public static class ProductOptionMapping
    {
        public static IReadOnlyList<CatalogOption> ToDomainOptions(this SaveProductInput input) =>
            input.Options!.Select(option => new CatalogOption(option.Id, option.Name,
                option.Values.Select(value => new CatalogOptionValue(value.Id, value.Value)).ToList())).ToList();

        public static IReadOnlyList<CatalogVariantValue> ToDomainVariants(this SaveProductInput input) =>
            input.Variants.Select(variant => new CatalogVariantValue(
                variant.Id, variant.Name, variant.Sku, variant.Price, variant.CompareAtPrice,
                variant.IsOffered, variant.Selections!.Select(s => new CatalogSelection(s.OptionId, s.ValueId)).ToList())).ToList();
    }
}
