using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Stores.Queries.GetStoreSetupStatus
{
    public sealed record StoreSetupSectionDto(
        string Key,
        string Label,
        bool IsRequired,
        bool IsComplete,
        IReadOnlyList<string> MissingFields,
        object? Data = null);

    public sealed record StoreSetupStatusDto(
        Guid StoreId,
        string Status,
        IReadOnlyList<StoreSetupSectionDto> Sections,
        int CompletedCount,
        int TotalCount,
        int RequiredCompletedCount,
        int RequiredTotalCount,
        bool CanPublish,
        bool CanAcceptOrders,
        string? NextRecommendedSection);
}
