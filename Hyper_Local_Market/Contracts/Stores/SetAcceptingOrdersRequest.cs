using System.ComponentModel.DataAnnotations;

namespace HyperLocalMarket.Api.Contracts.Stores
{
    public sealed record SetAcceptingOrdersRequest(
        [Required] bool? IsAcceptingOrders);
}
