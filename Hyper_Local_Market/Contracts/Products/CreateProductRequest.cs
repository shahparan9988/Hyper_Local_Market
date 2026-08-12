namespace HyperLocalMarket.Api.Contracts.Products
{
    public sealed record CreateProductRequest(
        Guid StoreId,
        string Name,
        string? Description,
        string? BrandName);
}
