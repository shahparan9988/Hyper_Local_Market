namespace HyperLocalMarket.Application.Common.TimeZones;

public static class SupportedStoreTimeZones
{
    private static readonly IReadOnlyDictionary<
        string,
        IReadOnlySet<string>>
        TimeZonesByCountry =
            new Dictionary<
                string,
                IReadOnlySet<string>>(
                StringComparer.OrdinalIgnoreCase)
            {
                ["BD"] = new HashSet<string>(
                    StringComparer.Ordinal)
                {
                    "Asia/Dhaka"
                },

                ["AU"] = new HashSet<string>(
                    StringComparer.Ordinal)
                {
                    "Australia/Sydney",
                    "Australia/Brisbane",
                    "Australia/Adelaide",
                    "Australia/Darwin",
                    "Australia/Perth",
                    "Australia/Hobart"
                }
            };

    public static bool IsSupported(
        string? timeZoneId)
    {
        if (string.IsNullOrWhiteSpace(timeZoneId))
        {
            return false;
        }

        return TimeZonesByCountry.Values.Any(
            timeZones =>
                timeZones.Contains(timeZoneId));
    }

    public static bool IsAllowedForCountry(
        string countryCode,
        string timeZoneId)
    {
        return
            TimeZonesByCountry.TryGetValue(
                countryCode,
                out var timeZones)
            &&
            timeZones.Contains(timeZoneId);
    }
}