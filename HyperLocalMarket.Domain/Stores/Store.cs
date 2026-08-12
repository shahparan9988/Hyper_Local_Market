using HyperLocalMarket.Domain.common;
using HyperLocalMarket.Domain.Products;
using HyperLocalMarket.Domain.Stores.Events;
using HyperLocalMarket.Domain.ValueObjects;

namespace HyperLocalMarket.Domain.Stores;

public sealed class Store : AggregateRoot
{
    private const int NameMaxLength = 200;
    private const int SlugMaxLength = 200;
    private const int DescriptionMaxLength = 2000;
    private const int PhoneNumberMaxLength = 30;
    private const int EmailMaxLength = 320;
    private const int ImageUrlMaxLength = 1000;
    private const int TimeZoneIdMaxLength = 100;

    private readonly List<StoreBusinessHour> _businessHours = new();

    public Guid UserId { get; private set; }

    public string Name { get; private set; } = default!;
    public string Slug { get; private set; } = default!;
    public string? Description { get; private set; }

    public string? PhoneNumber { get; private set; }
    public string? Email { get; private set; }

    public StoreLocation Location { get; private set; } = default!;
    public string TimeZoneId { get; private set; } = default!;

    public string? LogoUrl { get; private set; }
    public string? CoverImageUrl { get; private set; }

    public bool IsPickupAvailable { get; private set; }
    public bool IsDeliveryAvailable { get; private set; }
    public bool IsAcceptingOrders { get; private set; }

    public decimal? MinimumOrderAmount { get; private set; }
    public decimal? DeliveryFee { get; private set; }
    public decimal? DeliveryRadiusKm { get; private set; }

    public StoreStatus Status { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }


    public IReadOnlyCollection<StoreBusinessHour> BusinessHours =>
        _businessHours.AsReadOnly();

    private Store()
    {
    }

    private Store(
        Guid userId,
        string name,
        string slug,
        string? description,
        string? phoneNumber,
        string? email,
        StoreLocation location,
        string timeZoneId)
    {
        Id = Guid.NewGuid();

        UserId = ValidateUserId(userId);

        Name = ValidateRequiredText(
            name,
            nameof(name),
            NameMaxLength);

        Slug = ValidateSlug(slug);

        Description = ValidateOptionalText(
            description,
            nameof(description),
            DescriptionMaxLength);

        PhoneNumber = ValidateOptionalText(
            phoneNumber,
            nameof(phoneNumber),
            PhoneNumberMaxLength);

        Email = ValidateOptionalText(
            email,
            nameof(email),
            EmailMaxLength);

        Location = location
            ?? throw new ArgumentNullException(nameof(location));

        TimeZoneId = ValidateTimeZoneId(timeZoneId);

        Status = StoreStatus.Draft;

        IsPickupAvailable = true;
        IsDeliveryAvailable = false;
        IsAcceptingOrders = false;

        CreatedAtUtc = DateTime.UtcNow;
    }

    public static Store Create(
        Guid userId,
        string name,
        string slug,
        string? description,
        string? phoneNumber,
        string? email,
        StoreLocation location,
        string timeZoneId)
    {
        var store = new Store(
            userId,
            name,
            slug,
            description,
            phoneNumber,
            email,
            location,
            timeZoneId);

        store.AddDomainEvent(
            new StoreCreatedDomainEvent(
                store.Name,
                store.Id,
                store.UserId,
                store.CreatedAtUtc));

        return store;
    }

    public void UpdateBasicInfo(
        string name,
        string? description,
        string? phoneNumber,
        string? email)
    {
        Name = ValidateRequiredText(
            name,
            nameof(name),
            NameMaxLength);

        Description = ValidateOptionalText(
            description,
            nameof(description),
            DescriptionMaxLength);

        PhoneNumber = ValidateOptionalText(
            phoneNumber,
            nameof(phoneNumber),
            PhoneNumberMaxLength);

        Email = ValidateOptionalText(
            email,
            nameof(email),
            EmailMaxLength);

        MarkAsUpdated();
    }

    public void UpdateSlug(string slug)
    {
        Slug = ValidateSlug(slug);
        MarkAsUpdated();
    }

    public void UpdateAddress(StoreLocation location)
    {
        Location = location
            ?? throw new ArgumentNullException(nameof(location));

        MarkAsUpdated();
    }

    public void UpdateTimeZone(string timeZoneId)
    {
        TimeZoneId = ValidateTimeZoneId(timeZoneId);
        MarkAsUpdated();
    }

    public void SetBusinessHours(
        IEnumerable<StoreBusinessHour> businessHours)
    {
        ArgumentNullException.ThrowIfNull(businessHours);

        var normalizedHours = businessHours.ToList();

        if (normalizedHours.Any(x => x is null))
        {
            throw new ArgumentException(
                "Business hours cannot contain a null entry.",
                nameof(businessHours));
        }

        ValidateBusinessHours(normalizedHours);

        if (Status == StoreStatus.Active &&
            normalizedHours.Count == 0)
        {
            throw new InvalidOperationException(
                "An active store must have business hours.");
        }

        _businessHours.Clear();

        _businessHours.AddRange(
            normalizedHours
                .OrderBy(x => x.DayOfWeek)
                .ThenBy(x => x.OpensAt));

        MarkAsUpdated();
    }

    public void UpdateImages(
        string? logoUrl,
        string? coverImageUrl)
    {
        LogoUrl = ValidateOptionalText(
            logoUrl,
            nameof(logoUrl),
            ImageUrlMaxLength);

        CoverImageUrl = ValidateOptionalText(
            coverImageUrl,
            nameof(coverImageUrl),
            ImageUrlMaxLength);

        MarkAsUpdated();
    }

    public void UpdateFulfillment(
        bool isPickupAvailable,
        bool isDeliveryAvailable,
        decimal? minimumOrderAmount,
        decimal? deliveryFee,
        decimal? deliveryRadiusKm)
    {
        if (minimumOrderAmount is < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(minimumOrderAmount),
                "Minimum order amount cannot be negative.");
        }

        if (deliveryFee is < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(deliveryFee),
                "Delivery fee cannot be negative.");
        }

        if (deliveryRadiusKm is <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(deliveryRadiusKm),
                "Delivery radius must be greater than zero.");
        }

        if (isDeliveryAvailable && !deliveryRadiusKm.HasValue)
        {
            throw new InvalidOperationException(
                "A delivery radius is required when delivery is available.");
        }

        if (!isDeliveryAvailable &&
            (minimumOrderAmount.HasValue ||
             deliveryFee.HasValue ||
             deliveryRadiusKm.HasValue))
        {
            throw new InvalidOperationException(
                "Delivery settings cannot be provided when delivery " +
                "is unavailable.");
        }

        if (Status == StoreStatus.Active &&
            !isPickupAvailable &&
            !isDeliveryAvailable)
        {
            throw new InvalidOperationException(
                "An active store must provide pickup, delivery, or both.");
        }

        IsPickupAvailable = isPickupAvailable;
        IsDeliveryAvailable = isDeliveryAvailable;

        MinimumOrderAmount = minimumOrderAmount;
        DeliveryFee = deliveryFee;
        DeliveryRadiusKm = deliveryRadiusKm;

        MarkAsUpdated();
    }

    public void Activate()
    {
        if (Status == StoreStatus.Closed)
        {
            throw new InvalidOperationException(
                "A closed store cannot be activated.");
        }

        if (!IsPickupAvailable && !IsDeliveryAvailable)
        {
            throw new InvalidOperationException(
                "An active store must provide pickup, delivery, or both.");
        }

        if (_businessHours.Count == 0)
        {
            throw new InvalidOperationException(
                "Business hours must be configured before activation.");
        }

        if (IsDeliveryAvailable && !DeliveryRadiusKm.HasValue)
        {
            throw new InvalidOperationException(
                "A delivery radius must be configured before activation.");
        }

        Status = StoreStatus.Active;
        IsAcceptingOrders = true;

        MarkAsUpdated();
    }

    public void PauseOrders()
    {
        if (Status != StoreStatus.Active)
        {
            throw new InvalidOperationException(
                "Only an active store can pause orders.");
        }

        if (!IsAcceptingOrders)
        {
            return;
        }

        IsAcceptingOrders = false;
        MarkAsUpdated();
    }

    public void ResumeOrders()
    {
        if (Status != StoreStatus.Active)
        {
            throw new InvalidOperationException(
                "Only an active store can accept orders.");
        }

        if (_businessHours.Count == 0)
        {
            throw new InvalidOperationException(
                "Business hours must be configured before accepting orders.");
        }

        if (IsAcceptingOrders)
        {
            return;
        }

        IsAcceptingOrders = true;
        MarkAsUpdated();
    }

    public bool CanAcceptOrdersAt(DateTime utcDateTime)
    {
        if (utcDateTime.Kind != DateTimeKind.Utc)
        {
            throw new ArgumentException(
                "The supplied date and time must be UTC.",
                nameof(utcDateTime));
        }

        if (Status != StoreStatus.Active ||
            !IsAcceptingOrders)
        {
            return false;
        }

        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(
            TimeZoneId);

        var localDateTime = TimeZoneInfo.ConvertTimeFromUtc(
            utcDateTime,
            timeZone);

        var localTime = TimeOnly.FromDateTime(localDateTime);

        return _businessHours.Any(x =>
            x.DayOfWeek == localDateTime.DayOfWeek &&
            x.Contains(localTime));
    }

    public void Suspend()
    {
        if (Status == StoreStatus.Closed)
        {
            throw new InvalidOperationException(
                "A closed store cannot be suspended.");
        }

        Status = StoreStatus.Suspended;
        IsAcceptingOrders = false;

        MarkAsUpdated();
    }

    public void Close()
    {
        if (Status == StoreStatus.Closed)
        {
            return;
        }

        Status = StoreStatus.Closed;
        IsAcceptingOrders = false;

        MarkAsUpdated();
    }

    private void MarkAsUpdated()
    {
        UpdatedAtUtc = DateTime.UtcNow;
    }

    private static void ValidateBusinessHours(
        IReadOnlyCollection<StoreBusinessHour> businessHours)
    {
        foreach (var dayGroup in businessHours.GroupBy(x => x.DayOfWeek))
        {
            var periods = dayGroup.ToList();

            if (periods.Any(x => x.IsOpen24Hours) &&
                periods.Count > 1)
            {
                throw new ArgumentException(
                    $"{dayGroup.Key} cannot contain other opening periods " +
                    "when it is configured as open 24 hours.",
                    nameof(businessHours));
            }

            var orderedPeriods = periods
                .Where(x => !x.IsOpen24Hours)
                .OrderBy(x => x.OpensAt)
                .ToList();

            for (var index = 1;
                 index < orderedPeriods.Count;
                 index++)
            {
                var previous = orderedPeriods[index - 1];
                var current = orderedPeriods[index];

                var currentOpensAt = current.OpensAt
                    ?? throw new InvalidOperationException(
                        "A regular business period must have an opening time.");

                var previousClosesAt = previous.ClosesAt
                    ?? throw new InvalidOperationException(
                        "A regular business period must have a closing time.");

                if (currentOpensAt < previousClosesAt)
                {
                    throw new ArgumentException(
                        $"Business hours overlap on {dayGroup.Key}.",
                        nameof(businessHours));
                }
            }
        }
    }

    private static Guid ValidateUserId(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException(
                "User id is required.",
                nameof(userId));
        }

        return userId;
    }

    private static string ValidateSlug(string? slug)
    {
        var normalizedSlug = ValidateRequiredText(
            slug,
            nameof(slug),
            SlugMaxLength).ToLowerInvariant();

        if (normalizedSlug.StartsWith('-') ||
            normalizedSlug.EndsWith('-'))
        {
            throw new ArgumentException(
                "Slug cannot start or end with a hyphen.",
                nameof(slug));
        }

        if (normalizedSlug.Contains("--", StringComparison.Ordinal))
        {
            throw new ArgumentException(
                "Slug cannot contain consecutive hyphens.",
                nameof(slug));
        }

        var containsInvalidCharacter = normalizedSlug.Any(character =>
            !IsLowercaseAsciiLetter(character) &&
            !char.IsAsciiDigit(character) &&
            character != '-');

        if (containsInvalidCharacter)
        {
            throw new ArgumentException(
                "Slug can contain only lowercase letters, numbers, and hyphens.",
                nameof(slug));
        }

        return normalizedSlug;
    }

    private static bool IsLowercaseAsciiLetter(char character)
    {
        return character is >= 'a' and <= 'z';
    }

    private static string ValidateTimeZoneId(string? timeZoneId)
    {
        var normalizedTimeZoneId = ValidateRequiredText(
            timeZoneId,
            nameof(timeZoneId),
            TimeZoneIdMaxLength);

        try
        {
            _ = TimeZoneInfo.FindSystemTimeZoneById(
                normalizedTimeZoneId);
        }
        catch (TimeZoneNotFoundException exception)
        {
            throw new ArgumentException(
                $"Time zone '{normalizedTimeZoneId}' was not found.",
                nameof(timeZoneId),
                exception);
        }
        catch (InvalidTimeZoneException exception)
        {
            throw new ArgumentException(
                $"Time zone '{normalizedTimeZoneId}' is invalid.",
                nameof(timeZoneId),
                exception);
        }

        return normalizedTimeZoneId;
    }

    private static string ValidateRequiredText(
        string? value,
        string parameterName,
        int maximumLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(
                $"{parameterName} is required.",
                parameterName);
        }

        var normalizedValue = value.Trim();

        if (normalizedValue.Length > maximumLength)
        {
            throw new ArgumentException(
                $"{parameterName} cannot exceed " +
                $"{maximumLength} characters.",
                parameterName);
        }

        return normalizedValue;
    }

    private static string? ValidateOptionalText(
        string? value,
        string parameterName,
        int maximumLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var normalizedValue = value.Trim();

        if (normalizedValue.Length > maximumLength)
        {
            throw new ArgumentException(
                $"{parameterName} cannot exceed " +
                $"{maximumLength} characters.",
                parameterName);
        }

        return normalizedValue;
    }
}