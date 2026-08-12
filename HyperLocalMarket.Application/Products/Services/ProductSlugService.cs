using HyperLocalMarket.Application.Common.Abstractions;
using HyperLocalMarket.Application.Products.Repositories;
using HyperLocalMarket.Domain.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Products.Services
{
    public sealed class ProductSlugService : IProductSlugService
    {
        private readonly ISlugGenerator _slugGenerator;
        private readonly IProductRepository _productRepository;

        public ProductSlugService(
            ISlugGenerator slugGenerator,
            IProductRepository productRepository)
        {
            _slugGenerator = slugGenerator;
            _productRepository = productRepository;
        }

        public async Task<string> GenerateUniqueSlugAsync(
            Guid storeId,
            string productName,
            CancellationToken cancellationToken)
        {
            var baseSlug = _slugGenerator.Generate(
                productName,
                Product.SlugMaxLength);

            var candidate = baseSlug;

            var suffix = 2;

            while (await _productRepository.ExistsBySlugAsync(
                storeId,
                candidate,
                cancellationToken))
            {
                candidate = AddSuffix(
                    baseSlug,
                    suffix,
                    Product.SlugMaxLength);

                suffix++;
            }

            return candidate;
        }

        private static string AddSuffix(
            string baseSlug,
            int suffixNumber,
            int maxLength)
        {
            var suffix = $"-{suffixNumber}";

            var allowedBaseLength =
                maxLength - suffix.Length;

            var shortenedBase = baseSlug;

            if (shortenedBase.Length > allowedBaseLength)
            {
                shortenedBase =
                    shortenedBase[..allowedBaseLength]
                        .TrimEnd('-');
            }

            return $"{shortenedBase}{suffix}";
        }
    }
}
