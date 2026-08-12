using HyperLocalMarket.Domain.common;
using HyperLocalMarket.Domain.Inventory.Events;
using HyperLocalMarket.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Domain.Inventory
{
    public sealed class InventoryItem : AggregateRoot
    {
        private InventoryItem()
        {
        }

        private InventoryItem(
            Guid productVariantId,
            bool trackInventory,
            bool allowBackorder,
            decimal initialOnHandQuantity,
            decimal reorderPoint,
            DateTime utcNow)
        {
            Guard.NotEmpty(productVariantId, nameof(productVariantId));
            Guard.Utc(utcNow, nameof(utcNow));

            if (initialOnHandQuantity < 0)
            {
                throw new DomainException(
                    "Initial on-hand quantity cannot be negative.");
            }

            if (reorderPoint < 0)
            {
                throw new DomainException("Reorder point cannot be negative.");
            }

            ProductVariantId = productVariantId;
            TrackInventory = trackInventory;
            AllowBackorder = allowBackorder;
            OnHandQuantity = initialOnHandQuantity;
            ReservedQuantity = 0;
            ReorderPoint = reorderPoint;
            CreatedAtUtc = utcNow;
            UpdatedAtUtc = utcNow;
        }

        public Guid ProductVariantId { get; private set; }

        public bool TrackInventory { get; private set; }

        public bool AllowBackorder { get; private set; }

        public decimal OnHandQuantity { get; private set; }

        public decimal ReservedQuantity { get; private set; }

        public decimal ReorderPoint { get; private set; }

        public DateTime CreatedAtUtc { get; private set; }

        public DateTime UpdatedAtUtc { get; private set; }

        public decimal AvailableQuantity =>
            OnHandQuantity - ReservedQuantity;

        public bool IsOutOfStock =>
            TrackInventory &&
            !AllowBackorder &&
            AvailableQuantity <= 0;

        public bool IsLowStock =>
            TrackInventory &&
            AvailableQuantity > 0 &&
            AvailableQuantity <= ReorderPoint;

        public static InventoryItem Create(
            Guid productVariantId,
            bool trackInventory,
            bool allowBackorder,
            decimal initialOnHandQuantity,
            decimal reorderPoint,
            DateTime utcNow)
        {
            return new InventoryItem(
                productVariantId,
                trackInventory,
                allowBackorder,
                initialOnHandQuantity,
                reorderPoint,
                utcNow);
        }

        public void Receive(
            decimal quantity,
            string reason,
            DateTime utcNow)
        {
            EnsurePositive(quantity, nameof(quantity));
            var normalizedReason = Guard.RequiredText(reason, nameof(reason), 500);
            Guard.Utc(utcNow, nameof(utcNow));

            var previousOnHand = OnHandQuantity;
            OnHandQuantity += quantity;
            UpdatedAtUtc = utcNow;

            AddDomainEvent(
                new InventoryAdjustedDomainEvent(
                    Id,
                    ProductVariantId,
                    previousOnHand,
                    OnHandQuantity,
                    normalizedReason,
                    utcNow));
        }

        public void AdjustOnHand(
            decimal newOnHandQuantity,
            string reason,
            DateTime utcNow)
        {
            if (newOnHandQuantity < 0)
            {
                throw new DomainException(
                    "On-hand quantity cannot be negative.");
            }

            if (!AllowBackorder && newOnHandQuantity < ReservedQuantity)
            {
                throw new DomainException(
                    "On-hand quantity cannot be lower than reserved quantity.");
            }

            var normalizedReason = Guard.RequiredText(reason, nameof(reason), 500);
            Guard.Utc(utcNow, nameof(utcNow));

            var previousOnHand = OnHandQuantity;
            OnHandQuantity = newOnHandQuantity;
            UpdatedAtUtc = utcNow;

            AddDomainEvent(
                new InventoryAdjustedDomainEvent(
                    Id,
                    ProductVariantId,
                    previousOnHand,
                    newOnHandQuantity,
                    normalizedReason,
                    utcNow));
        }

        public void Reserve(
            Guid reservationId,
            decimal quantity,
            DateTime utcNow)
        {
            Guard.NotEmpty(reservationId, nameof(reservationId));
            EnsurePositive(quantity, nameof(quantity));
            Guard.Utc(utcNow, nameof(utcNow));

            if (TrackInventory &&
                !AllowBackorder &&
                AvailableQuantity < quantity)
            {
                throw new DomainException(
                    "Insufficient available inventory for this reservation.");
            }

            if (TrackInventory)
            {
                ReservedQuantity += quantity;
            }

            UpdatedAtUtc = utcNow;

            AddDomainEvent(
                new InventoryReservedDomainEvent(
                    Id,
                    ProductVariantId,
                    reservationId,
                    quantity,
                    utcNow));
        }

        public void ReleaseReservation(
            Guid reservationId,
            decimal quantity,
            DateTime utcNow)
        {
            Guard.NotEmpty(reservationId, nameof(reservationId));
            EnsurePositive(quantity, nameof(quantity));
            Guard.Utc(utcNow, nameof(utcNow));

            if (TrackInventory && ReservedQuantity < quantity)
            {
                throw new DomainException(
                    "Cannot release more inventory than is currently reserved.");
            }

            if (TrackInventory)
            {
                ReservedQuantity -= quantity;
            }

            UpdatedAtUtc = utcNow;

            AddDomainEvent(
                new InventoryReservationReleasedDomainEvent(
                    Id,
                    ProductVariantId,
                    reservationId,
                    quantity,
                    utcNow));
        }

        public void CommitReservation(
            Guid reservationId,
            decimal quantity,
            DateTime utcNow)
        {
            Guard.NotEmpty(reservationId, nameof(reservationId));
            EnsurePositive(quantity, nameof(quantity));
            Guard.Utc(utcNow, nameof(utcNow));

            if (TrackInventory && ReservedQuantity < quantity)
            {
                throw new DomainException(
                    "Cannot commit more inventory than is currently reserved.");
            }

            if (TrackInventory)
            {
                if (!AllowBackorder && OnHandQuantity < quantity)
                {
                    throw new DomainException(
                        "Insufficient on-hand inventory to commit the reservation.");
                }

                ReservedQuantity -= quantity;
                OnHandQuantity -= quantity;
            }

            UpdatedAtUtc = utcNow;

            AddDomainEvent(
                new InventoryReservationCommittedDomainEvent(
                    Id,
                    ProductVariantId,
                    reservationId,
                    quantity,
                    utcNow));
        }

        public void ChangePolicy(
            bool trackInventory,
            bool allowBackorder,
            decimal reorderPoint,
            DateTime utcNow)
        {
            if (reorderPoint < 0)
            {
                throw new DomainException("Reorder point cannot be negative.");
            }

            Guard.Utc(utcNow, nameof(utcNow));

            if (!trackInventory && ReservedQuantity > 0)
            {
                throw new DomainException(
                    "Inventory tracking cannot be disabled while stock is reserved.");
            }

            TrackInventory = trackInventory;
            AllowBackorder = allowBackorder;
            ReorderPoint = reorderPoint;

            UpdatedAtUtc = utcNow;
        }

        private static void EnsurePositive(
            decimal quantity,
            string parameterName)
        {
            if (quantity <= 0)
            {
                throw new DomainException(
                    $"{parameterName} must be greater than zero.");
            }
        }

    }
}
