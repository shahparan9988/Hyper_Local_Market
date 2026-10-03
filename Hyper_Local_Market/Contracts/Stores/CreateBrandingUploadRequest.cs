namespace HyperLocalMarket.Api.Contracts.Stores
{
    public sealed record CreateBrandingUploadRequest(
        string FileName,
        string ContentType,
        long FileSizeBytes);
}
