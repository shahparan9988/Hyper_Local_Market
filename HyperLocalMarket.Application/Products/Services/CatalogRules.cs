using HyperLocalMarket.Application.Products.Repositories;
using HyperLocalMarket.Domain.Inventory;
using HyperLocalMarket.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Products.Services
{
    public static class CatalogRules
    {
        public static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);
        public static void Version(int actual, int? expected)
        {
            if (actual != expected) throw new ConflictException("This record changed. Reload before saving again.");
        }
        public static void InventoryVersion(InventoryItem? item, int? expected)
        {
            if (item?.Version != expected) throw new ConflictException("Stock changed. Reload before saving again.");
        }
        public static void Quantity(decimal value, decimal reserved = 0)
        {
            if (value < reserved || value < 0 || value > 999999999m || decimal.Round(value, 3) != value)
                throw new DomainException("Stock must be non-negative, at least the reserved amount, with at most three decimals.");
        }
        public static string Hash<T>(T input) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(input, Json))));
        public static T Replay<T>(CatalogReceipt receipt, string hash)
        {
            if (receipt.Hash != hash) throw new ConflictException("This request key has already been used with different values.");
            return JsonSerializer.Deserialize<T>(receipt.ResultJson, Json) ?? throw new InvalidOperationException("Invalid stored request receipt.");
        }
        public static HashSet<Guid> Descendants(IEnumerable<(Guid Id, Guid? ParentId)> nodes, Guid selected)
        {
            var children = nodes.Where(x => x.ParentId.HasValue).GroupBy(x => x.ParentId!.Value)
                .ToDictionary(x => x.Key, x => x.Select(v => v.Id).ToArray());
            var result = new HashSet<Guid>(); var stack = new Stack<Guid>(); stack.Push(selected);
            while (stack.TryPop(out var current))
            {
                if (!result.Add(current)) continue;
                if (children.TryGetValue(current, out var nested)) foreach (var child in nested) stack.Push(child);
            }
            return result;
        }
    }

}
