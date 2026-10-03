using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Domain.Products
{
    public enum ProductType { Product = 1, Service = 2 }
    public enum ProductAvailability { Available = 1, Unavailable = 2 }

    public sealed record CatalogOptionValue(Guid Id, string Value);
    public sealed record CatalogOption(Guid Id, string Name, IReadOnlyList<CatalogOptionValue> Values);
    public sealed record CatalogSelection(Guid OptionId, Guid ValueId);
    public sealed record CatalogVariantValue(
        Guid Id, string Name, string? Sku, decimal? Price, decimal? CompareAtPrice,
        bool IsOffered = true, IReadOnlyList<CatalogSelection>? Selections = null);
    public sealed record CatalogImageValue(Guid AssetId, string Url, bool IsLegacy = false);
}
