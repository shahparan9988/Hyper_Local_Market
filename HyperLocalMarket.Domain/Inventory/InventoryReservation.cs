using HyperLocalMarket.Domain.common;
using HyperLocalMarket.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Domain.Inventory
{
    public sealed class InventoryReservation : AggregateRoot
    {
        private InventoryReservation()
        {
        }

        private InventoryReservation(
            Guid inventoryItemId,
            Guid productVariantId,
            Guid referenceId,
            decimal quantity,
            DateTime expiresAtUtc,
            DateTime utcNow)
        {
            Guard.NotEmpty(inventoryItemId, nameof(inventoryItemId));
            Guard.NotEmpty(productVariantId, nameof(productVariantId));
            Guard.NotEmpty(referenceId, nameof(referenceId));
            Guard.Utc(expiresAtUtc, nameof(expiresAtUtc));
            Guard.Utc(utcNow, nameof(utcNow));

            if (quantity <= 0)
            {
                throw new DomainException(
                    "Reservation quantity must be greater than zero.");
            }

            if (expiresAtUtc <= utcNow)
            {
                throw new DomainException(
                    "Inventory reservation expiry must be in the future.");
            }

            InventoryItemId = inventoryItemId;
            ProductVariantId = productVariantId;
            ReferenceId = referenceId;
            Quantity = quantity;
            ExpiresAtUtc = expiresAtUtc;
            Status = InventoryReservationStatus.Active;
            CreatedAtUtc = utcNow;
            UpdatedAtUtc = utcNow;
        }

        public Guid InventoryItemId { get; private set; }

        public Guid ProductVariantId { get; private set; }

        public Guid ReferenceId { get; private set; }

        public decimal Quantity { get; private set; }

        public DateTime ExpiresAtUtc { get; private set; }

        public InventoryReservationStatus Status { get; private set; }

        public DateTime CreatedAtUtc { get; private set; }

        public DateTime UpdatedAtUtc { get; private set; }

        public DateTime? CompletedAtUtc { get; private set; }

        public bool IsActive => Status == InventoryReservationStatus.Active;

        public static InventoryReservation Create(
            Guid inventoryItemId,
            Guid productVariantId,
            Guid referenceId,
            decimal quantity,
            DateTime expiresAtUtc,
            DateTime utcNow)
        {
            return new InventoryReservation(
                inventoryItemId,
                productVariantId,
                referenceId,
                quantity,
                expiresAtUtc,
                utcNow);
        }

        public void Commit(DateTime utcNow)
        {
            EnsureActive();
            Guard.Utc(utcNow, nameof(utcNow));

            if (utcNow >= ExpiresAtUtc)
            {
                throw new DomainException(
                    "An expired inventory reservation cannot be committed.");
            }

            Status = InventoryReservationStatus.Committed;
            CompletedAtUtc = utcNow;
            UpdatedAtUtc = utcNow;
        }

        public void Release(DateTime utcNow)
        {
            EnsureActive();
            Guard.Utc(utcNow, nameof(utcNow));

            Status = InventoryReservationStatus.Released;
            CompletedAtUtc = utcNow;
            UpdatedAtUtc = utcNow;
        }

        public void Expire(DateTime utcNow)
        {
            EnsureActive();
            Guard.Utc(utcNow, nameof(utcNow));

            if (utcNow < ExpiresAtUtc)
            {
                throw new DomainException(
                    "Inventory reservation cannot expire before its expiry time.");
            }

            Status = InventoryReservationStatus.Expired;
            CompletedAtUtc = utcNow;
            UpdatedAtUtc = utcNow;
        }

        private void EnsureActive()
        {
            if (!IsActive)
            {
                throw new DomainException(
                    "Only an active inventory reservation can be changed.");
            }
        }
    }
}
