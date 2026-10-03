using HyperLocalMarket.Domain.common;
using HyperLocalMarket.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Domain.Products
{
    public static class ProductOptionSet
    {
        public const int MaximumValuesPerOption = MaximumCombinations;
        public const int MaximumCombinations = 1000;

        public static void Validate(
            IReadOnlyList<CatalogOption> options,
            IReadOnlyList<CatalogVariantValue> variants)
        {
            if (options.Count > Product.MaximumOptionCount)
                throw new DomainException($"Use at most {Product.MaximumOptionCount} options.");
            if (variants.Count is < 1 or > MaximumCombinations)
                throw new DomainException($"Supply 1–{MaximumCombinations} variants.");
            if (options.Count == 0 && variants.Count != 1)
                throw new DomainException("A product without options must have one standard variant.");

            var optionIds = new HashSet<Guid>();
            var valueIds = new HashSet<Guid>();
            var optionNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            long combinationCount = 1;
            foreach (var option in options)
            {
                Guard.NotEmpty(option.Id, nameof(option.Id));
                var name = Guard.RequiredText(option.Name, nameof(option.Name), ProductOption.NameMaxLength);
                if (!optionIds.Add(option.Id) || !optionNames.Add(name))
                    throw new DomainException("Option IDs and names must be unique.");
                if (option.Values.Count is < 1 or > MaximumValuesPerOption)
                    throw new DomainException($"Each option needs 1–{MaximumValuesPerOption} values.");
                var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (var value in option.Values)
                {
                    Guard.NotEmpty(value.Id, nameof(value.Id));
                    var text = Guard.RequiredText(value.Value, nameof(value.Value), ProductOptionValue.ValueMaxLength);
                    if (!valueIds.Add(value.Id) || !names.Add(text))
                        throw new DomainException("Value IDs must be unique, and values cannot repeat within an option.");
                }
                combinationCount *= option.Values.Count;
                if (combinationCount > MaximumCombinations)
                    throw new DomainException($"Use at most {MaximumCombinations} possible combinations.");
            }

            var combinations = new HashSet<string>(StringComparer.Ordinal);
            foreach (var variant in variants)
            {
                var selections = variant.Selections ?? [];
                if (selections.Count != options.Count ||
                    selections.Select(x => x.OptionId).Distinct().Count() != options.Count)
                    throw new DomainException("Each variant must choose exactly one value from every option.");
                foreach (var option in options)
                {
                    var selection = selections.SingleOrDefault(x => x.OptionId == option.Id);
                    if (selection is null || !option.Values.Any(x => x.Id == selection.ValueId))
                        throw new DomainException($"Choose a valid value for '{option.Name}'.");
                }
                if (!combinations.Add(Signature(selections)))
                    throw new DomainException("Two variants cannot use the same option combination.");
                Guard.RequiredText(Name(options, selections), "variantName", ProductVariant.DisplayNameMaxLength);
            }
        }

        public static string Signature(IEnumerable<CatalogSelection> selections) =>
            string.Join("|", selections.OrderBy(x => x.OptionId).Select(x => $"{x.OptionId:N}:{x.ValueId:N}"));

        public static string Name(IReadOnlyList<CatalogOption> options, IReadOnlyList<CatalogSelection> selections) =>
            options.Count == 0 ? "Standard" : string.Join(" / ", options.Select(option =>
                option.Values.Single(value => value.Id == selections.Single(s => s.OptionId == option.Id).ValueId).Value.Trim()));
    }

}
