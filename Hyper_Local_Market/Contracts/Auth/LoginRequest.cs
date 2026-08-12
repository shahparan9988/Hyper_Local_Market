namespace HyperLocalMarket.Api.Contracts.Auth
{
    public sealed record LoginRequest(
    string EmailOrPhone,
    string Password);
}
