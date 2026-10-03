namespace HyperLocalMarket.Api.Contracts.Stores
{
    public sealed record UpdateBusinessInfoRequest(
        string? Name,
        string? Description);
}
