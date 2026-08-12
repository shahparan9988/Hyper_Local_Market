namespace HyperLocalMarket.Api.Contracts.Auth
{
    public sealed record RegisterRequest(
        string Phone,
        string Email,
        string Password);
    
}
