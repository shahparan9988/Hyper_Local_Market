using HyperLocalMarket.Application.Common.Interfaces.Persistence;
using HyperLocalMarket.Application.Products.Repositories;
using HyperLocalMarket.Application.Products.Services;
using HyperLocalMarket.Application.Stores.Repositories;
using HyperLocalMarket.Domain.Products;
using HyperLocalMarket.Shared.Exceptions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Products.Commands.CreateProduct
{
    public sealed class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
    {
        private readonly IProductRepository _productRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly IProductSlugService _productSlugService;
        //private readonly ICategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TimeProvider _timeProvider;

        public CreateProductCommandHandler(
            IProductRepository productRepository,
            IStoreRepository storeRepository,
            IProductSlugService productSlugService,
            //ICategoryRepository categoryRepository,
            IUnitOfWork unitOfWork,
            TimeProvider timeProvider)
        {
            _productRepository = productRepository;
            _storeRepository = storeRepository;
            _productSlugService = productSlugService;
            //_categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
            _timeProvider = timeProvider;
        }

        public async Task<Guid> Handle(
            CreateProductCommand command,
            CancellationToken cancellationToken)
        {
            var store = await _storeRepository.GetByIdAndUserIdAsync(
                command.StoreId,
                command.UserId,
                cancellationToken);

            if (store is null)
            {
                throw new NotFoundException(
                    "This store name is not found in the system");
            }

            var duplicateProductExists =
                await _productRepository.ExistsByStoreAndNameAsync(
                    command.StoreId,
                    command.Name.Trim(),
                    cancellationToken);

            if (duplicateProductExists)
            {
                throw new ConflictException(
                    $"A product named '{command.Name.Trim()}' already exists " +
                    $"in this store.");
            }

            //if (command.CategoryId.HasValue)
            //{
            //    var category = await _categoryRepository.GetByIdAsync(
            //        command.CategoryId.Value,
            //        cancellationToken);

            //    if (category is null)
            //    {
            //        throw new NotFoundException(
            //            "This category is not found in the system!");
            //    }

                // If your Category has a status, validate it here.
                // For example:
                //
                // if (!category.CanBeAssignedToProduct)
                // {
                //     throw new ConflictException(
                //         "The selected category cannot be assigned to a product.");
                // }
            //}

            var utcNow = _timeProvider.GetUtcNow().UtcDateTime;


            var slug = await _productSlugService
                .GenerateUniqueSlugAsync(
                    command.StoreId,
                    command.Name,
                    cancellationToken);

            var product = Product.Create(
                storeId: command.StoreId,
                name: command.Name,
                slug: slug,
                description: command.Description,
                brand: command.BrandName,
                utcNow: utcNow);

            //if (command.CategoryId.HasValue)
            //{
            //    product.SelectCategory(
            //        command.CategoryId.Value,
            //        utcNow);
            //}

            await _productRepository.AddAsync(
                product,
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);

            return product.Id;
        }
    }
}
