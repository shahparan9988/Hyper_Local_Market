using HyperLocalMarket.Application.Common.Interfaces.Persistence;
using HyperLocalMarket.Application.Products.Repositories;
using HyperLocalMarket.Application.Stores.Dtos;
using HyperLocalMarket.Application.Stores.Repositories;
using HyperLocalMarket.Domain.Products;
using HyperLocalMarket.Domain.Stores;
using HyperLocalMarket.Shared.Exceptions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Stores.Commands.DeliveryOptions
{
    // MediatR registers every implemented request-handler interface.
    public sealed class DeliveryOptionHandlers :
        IRequestHandler<GetDeliveryOptionsQuery, IReadOnlyList<DeliveryOptionDto>>,
        IRequestHandler<CreateDeliveryOptionCommand, DeliveryOptionDto>,
        IRequestHandler<UpdateDeliveryOptionCommand, DeliveryOptionDto>,
        IRequestHandler<SetDeliveryOptionStatusCommand, DeliveryOptionDto>,
        IRequestHandler<SaveFulfillmentCommand, FulfillmentDto>,
        IRequestHandler<GetProductDeliverySelectionQuery, ProductDeliverySelectionDto>,
        IRequestHandler<SaveProductDeliverySelectionCommand, ProductDeliverySelectionDto>,
        IRequestHandler<GetPublicProductDeliveryQuery, PublicProductDeliveryDto?>
    {
        private readonly IStoreRepository _stores;
        private readonly IDeliveryOptionRepository _options;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TimeProvider _time;
        private readonly IProductCatalogRepository _catalog;

        public DeliveryOptionHandlers(IStoreRepository stores, IDeliveryOptionRepository options,
            IUnitOfWork unitOfWork, TimeProvider time, IProductCatalogRepository catalog)
        {
            _stores = stores;
            _options = options;
            _unitOfWork = unitOfWork;
            _time = time;
            _catalog = catalog;
        }

        private async Task RequireOwner(Guid storeId, Guid userId, CancellationToken ct)
        {
            if (await _stores.GetByIdAndUserIdAsync(storeId, userId, ct) is null)
                throw new NotFoundException("Store was not found.");
        }

        private static DeliveryFeeType ParseFeeType(string? value) => value switch
        {
            "Fixed" => DeliveryFeeType.Fixed,
            "StartingFrom" => DeliveryFeeType.StartingFrom,
            "ContactSeller" => DeliveryFeeType.ContactSeller,
            _ => throw new DomainException("Invalid delivery fee type.")
        };

        private static void CheckVersion(int actual, int expected)
        {
            if (actual != expected)
                throw new ConflictException("This information changed. Reload before saving again.");
        }

        public async Task<IReadOnlyList<DeliveryOptionDto>> Handle(
            GetDeliveryOptionsQuery request, CancellationToken ct)
        {
            await RequireOwner(request.StoreId, request.UserId, ct);
            return (await _options.ListAsync(request.StoreId, ct))
                .Select(DeliveryOptionDto.From).ToList();
        }

        public async Task<DeliveryOptionDto> Handle(
            CreateDeliveryOptionCommand request, CancellationToken ct)
        {
            await RequireOwner(request.StoreId, request.UserId, ct);
            var x = request.Input;
            var option = StoreDeliveryOption.Create(request.StoreId, x.Name,
                x.CoverageDescription, x.EstimatedTimeDescription, ParseFeeType(x.FeeType),
                x.FeeAmount, x.CurrencyCode, x.Conditions, x.IsActive,
                _time.GetUtcNow().UtcDateTime);
            await _options.AddAsync(option, ct);
            await _unitOfWork.SaveChangesAsync(ct);
            return DeliveryOptionDto.From(option);
        }

        public async Task<DeliveryOptionDto> Handle(
            UpdateDeliveryOptionCommand request, CancellationToken ct)
        {
            await RequireOwner(request.StoreId, request.UserId, ct);
            var option = await _options.GetTrackedAsync(request.StoreId, request.OptionId, ct)
                ?? throw new NotFoundException("Delivery option was not found.");
            CheckVersion(option.Version, request.ExpectedVersion);
            var x = request.Input;
            option.Update(x.Name, x.CoverageDescription, x.EstimatedTimeDescription,
                ParseFeeType(x.FeeType), x.FeeAmount, x.CurrencyCode, x.Conditions,
                x.IsActive, _time.GetUtcNow().UtcDateTime);
            await _unitOfWork.SaveChangesAsync(ct);
            return DeliveryOptionDto.From(option);
        }

        public async Task<DeliveryOptionDto> Handle(
            SetDeliveryOptionStatusCommand request, CancellationToken ct)
        {
            await RequireOwner(request.StoreId, request.UserId, ct);
            var option = await _options.GetTrackedAsync(request.StoreId, request.OptionId, ct)
                ?? throw new NotFoundException("Delivery option was not found.");
            CheckVersion(option.Version, request.ExpectedVersion);
            option.SetActive(request.IsActive, _time.GetUtcNow().UtcDateTime);
            await _unitOfWork.SaveChangesAsync(ct);
            return DeliveryOptionDto.From(option);
        }

        public async Task<FulfillmentDto> Handle(SaveFulfillmentCommand request, CancellationToken ct)
        {
            var store = await _stores.GetByIdAndUserIdAsTrackingAsync(
                request.StoreId, request.UserId, ct)
                ?? throw new NotFoundException("Store was not found.");
            store.UpdateFulfillment(request.IsPickupAvailable, request.IsDeliveryAvailable,
                request.FulfillmentNotes, _time.GetUtcNow().UtcDateTime);
            await _unitOfWork.SaveChangesAsync(ct);
            return new FulfillmentDto(store.Id, store.IsPickupAvailable,
                store.IsDeliveryAvailable, store.FulfillmentNotes, store.FulfillmentConfiguredAtUtc);
        }

        public async Task<ProductDeliverySelectionDto> Handle(
            GetProductDeliverySelectionQuery request, CancellationToken ct)
        {
            await RequireOwner(request.StoreId, request.UserId, ct);
            var product = await _options.GetProductTrackedAsync(request.StoreId, request.ProductId, ct)
                ?? throw new NotFoundException("Product was not found.");
            var options = await _options.ListAsync(request.StoreId, ct);
            return new ProductDeliverySelectionDto(product.Id, product.DeliveryOptionsVersion,
                product.DeliveryOptions.Select(x => x.DeliveryOptionId).ToList(),
                options.Select(DeliveryOptionDto.From).ToList());
        }

        public async Task<ProductDeliverySelectionDto> Handle(
            SaveProductDeliverySelectionCommand request, CancellationToken ct)
        {
            await using var transaction = await _catalog.BeginOwnerWriteAsync(request.StoreId, request.UserId, ct);
            var product = await _options.GetProductTrackedAsync(request.StoreId, request.ProductId, ct)
                ?? throw new NotFoundException("Product was not found.");
            CheckVersion(product.DeliveryOptionsVersion, request.ExpectedVersion);
            var ids = request.DeliveryOptionIds ?? throw new DomainException("DeliveryOptionIds is required; use [] to clear.");
            if (ids.Count > 50 || ids.Any(id => id == Guid.Empty) || ids.Distinct().Count() != ids.Count)
                throw new DomainException("Select at most 50 distinct delivery options.");
            if (ids.Count > 0 && (product.Type == ProductType.Service || !transaction.Store.IsDeliveryAvailable))
                throw new DomainException("Delivery is not available for this listing.");
            var selected = await _catalog.LockDeliveryOptionsAsync(request.StoreId, ids, ct);
            if (selected.Count != ids.Count || selected.Any(x => !x.IsActive))
                throw new DomainException("A selected delivery method is inactive or belongs to another store.");
            product.ReplaceDeliveryOptions(ids, _time.GetUtcNow().UtcDateTime);
            await _unitOfWork.SaveChangesAsync(ct);
            var options = await _options.ListAsync(request.StoreId, ct);
            await transaction.CommitAsync(ct);
            return new ProductDeliverySelectionDto(product.Id, product.DeliveryOptionsVersion,
                product.DeliveryOptions.Select(x => x.DeliveryOptionId).ToList(), options.Select(DeliveryOptionDto.From).ToList());
        }

        public Task<PublicProductDeliveryDto?> Handle(
            GetPublicProductDeliveryQuery request, CancellationToken ct) =>
            _options.GetPublicAsync(request.ProductId, ct);
    }

}
