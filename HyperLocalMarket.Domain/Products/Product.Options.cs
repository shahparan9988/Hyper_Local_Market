using HyperLocalMarket.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Domain.Products
{
    public sealed partial class Product
    {
        public void ValidateCatalogOptions(
            IReadOnlyList<CatalogOption> options,
            IReadOnlyList<CatalogVariantValue> variants)
        {
            ProductOptionSet.Validate(options, variants);
            var currentOptions = _options.Where(x => x.IsListed).ToList();
            foreach (var option in options)
            {
                foreach (var value in option.Values)
                    if (_options.Any(old => old.Id != option.Id && old.Values.Any(x => x.Id == value.Id)))
                        throw new DomainException("An option value cannot move to another option.");
            }
            foreach (var incoming in variants)
            {
                var existing = _variants.SingleOrDefault(x => x.Id == incoming.Id);
                if (existing is null) continue;
                var previous = existing.Selections.Select(x => new CatalogSelection(x.ProductOptionId, x.ProductOptionValueId)).ToList();
                var selections = incoming.Selections ?? [];
                // One-time conversion from the previous flat, single-option editor.
                var legacyConversion = currentOptions.Count == 0 && previous.Count == 0 && options.Count <= 1;
                if (!legacyConversion && ProductOptionSet.Signature(previous) != ProductOptionSet.Signature(selections))
                    throw new DomainException("An existing variant cannot be changed into a different combination. Generate a new variant instead.");
            }
        }

        private void ApplyCatalogOptions(IReadOnlyList<CatalogOption> options)
        {
            foreach (var option in _options) option.StageCatalogEdit();
            for (var index = 0; index < options.Count; index++)
            {
                var input = options[index];
                var option = _options.SingleOrDefault(x => x.Id == input.Id);
                if (option is null) _options.Add(ProductOption.CreateCatalog(Id, input, index));
                else option.ApplyCatalog(input, index);
            }
        }
    }

}
