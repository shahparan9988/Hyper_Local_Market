namespace HyperLocalMarket.Api.Contracts.Stores
{
    public sealed record UpdateBusinessHoursRequest(
        IReadOnlyList<BusinessHoursDayRequest> Hours);

    public sealed record BusinessHoursDayRequest(
        string DayOfWeek,
        bool IsClosed,
        bool IsOpen24Hours,
        IReadOnlyList<BusinessHoursPeriodRequest> Periods);

    public sealed record BusinessHoursPeriodRequest(
        string OpensAt,
        string ClosesAt);
}
