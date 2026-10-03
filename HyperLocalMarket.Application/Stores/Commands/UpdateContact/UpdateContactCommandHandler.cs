using HyperLocalMarket.Application.Common.Abstractions;
using HyperLocalMarket.Application.Common.Interfaces.Persistence;
using HyperLocalMarket.Application.Common.PhoneNumbers;
using HyperLocalMarket.Application.Common.TimeZones;
using HyperLocalMarket.Application.Stores.Dtos;
using HyperLocalMarket.Application.Stores.Repositories;
using HyperLocalMarket.Shared.Exceptions;
using MediatR;

namespace HyperLocalMarket.Application.Stores.Commands.UpdateContact;

public sealed class UpdateContactCommandHandler
    : IRequestHandler<
        UpdateContactCommand,
        StoreContactDto?>
{
    private readonly IStoreRepository _storeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateContactCommandHandler(
        IStoreRepository storeRepository,
        IUnitOfWork unitOfWork)
    {
        _storeRepository = storeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<StoreContactDto?> Handle(
        UpdateContactCommand request,
        CancellationToken cancellationToken)
    {
        var store =
            await _storeRepository
                .GetByIdAndUserIdAsTrackingAsync(
                    request.StoreId,
                    request.UserId,
                    cancellationToken);

        if (store is null)
        {
            return null;
        }

        var timeZoneId =
            request.TimeZoneId!.Trim();

        /*
         * This requires loading the store because command
         * validation cannot determine the store's country.
         */
        if (!SupportedStoreTimeZones.IsAllowedForCountry(
                store.Location.CountryCode,
                timeZoneId))
        {
            throw new ValidationException(
                new Dictionary<string, string[]>
                {
                    ["timeZoneId"] =
                    [
                        "The timezone does not match the store location."
                    ]
                });
        }

        var normalizedPhoneNumber =
            SupportedPhoneNumber.Normalize(
                request.PhoneNumber!);

        store.UpdateContact(
            normalizedPhoneNumber,
            request.Email);

        store.UpdateTimeZone(timeZoneId);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return new StoreContactDto(
            store.Id,
            store.PhoneNumber!,
            store.Email,
            store.TimeZoneId);
    }
}