using HyperLocalMarket.Application.Products.Dtos;

namespace HyperLocalMarket.Api.Contracts.Products
{
    public sealed record SetProductStatusRequest(
        int ExpectedVersion,
        string Status);
    public sealed record SetProductAvailabilityRequest(
        int ExpectedVersion,
        string Availability);
    public sealed record AdjustProductInventoryRequest(
        int ExpectedVersion,
        string Reason,
        IReadOnlyList<InventoryVariantInput> Variants);
    public sealed record CreateStoreProductCategoryRequest(
        string Name,
        Guid? ParentId);
    public sealed record CreateProductImageUploadRequest(
        string FileName,
        string ContentType,
        long FileSizeBytes);

}
