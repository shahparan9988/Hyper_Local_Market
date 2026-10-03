using HyperLocalMarket.Domain.common;
using HyperLocalMarket.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Domain.Images
{
    public sealed class StoreBrandingImage : AggregateRoot
    {
        public const int ObjectKeyMaxLength = 1_000;
        public const int ContentTypeMaxLength = 100;
        public const int PublicUrlMaxLength = 2_000;
        public const int FailureReasonMaxLength = 500;

        private StoreBrandingImage()
        {
        }

        private StoreBrandingImage(
            Guid id,
            Guid storeId,
            StoreBrandingImageKind kind,
            string originalObjectKey,
            string contentType,
            long expectedSizeBytes,
            DateTime utcNow)
            : base(id)
        {
            Guard.NotEmpty(id, nameof(id));
            Guard.NotEmpty(storeId, nameof(storeId));
            Guard.Utc(utcNow, nameof(utcNow));

            StoreId = storeId;
            Kind = kind;

            BeginNewUpload(
                originalObjectKey,
                contentType,
                expectedSizeBytes,
                utcNow);

            CreatedAtUtc = utcNow;
        }

        public Guid StoreId { get; private set; }

        public StoreBrandingImageKind Kind { get; private set; }

        public ImageProcessingStatus Status { get; private set; }

        public string OriginalObjectKey { get; private set; } = null!;

        public string? ProcessedObjectKey { get; private set; }

        public string ContentType { get; private set; } = null!;

        public long ExpectedSizeBytes { get; private set; }

        public string? PublicUrl { get; private set; }

        public string? FailureReason { get; private set; }

        public DateTime CreatedAtUtc { get; private set; }

        public DateTime UpdatedAtUtc { get; private set; }

        public static StoreBrandingImage Create(
            Guid id,
            Guid storeId,
            StoreBrandingImageKind kind,
            string originalObjectKey,
            string contentType,
            long expectedSizeBytes,
            DateTime utcNow)
        {
            return new StoreBrandingImage(
                id,
                storeId,
                kind,
                originalObjectKey,
                contentType,
                expectedSizeBytes,
                utcNow);
        }

        public void BeginNewUpload(
            string originalObjectKey,
            string contentType,
            long expectedSizeBytes,
            DateTime utcNow)
        {
            Guard.Utc(utcNow, nameof(utcNow));

            if (!Enum.IsDefined(Kind))
            {
                throw new DomainException("Invalid store image kind.");
            }

            if (expectedSizeBytes <= 0)
            {
                throw new DomainException(
                    "Expected image size must be greater than zero.");
            }

            OriginalObjectKey = Guard.RequiredText(
                originalObjectKey,
                nameof(originalObjectKey),
                ObjectKeyMaxLength);

            ContentType = Guard.RequiredText(
                contentType,
                nameof(contentType),
                ContentTypeMaxLength);

            ExpectedSizeBytes = expectedSizeBytes;
            ProcessedObjectKey = null;
            PublicUrl = null;
            FailureReason = null;
            Status = ImageProcessingStatus.PendingUpload;
            UpdatedAtUtc = utcNow;
        }

        public void MarkProcessing(DateTime utcNow)
        {
            Guard.Utc(utcNow, nameof(utcNow));

            if (Status is ImageProcessingStatus.Ready
                or ImageProcessingStatus.Removed)
            {
                return;
            }

            Status = ImageProcessingStatus.Processing;
            FailureReason = null;
            UpdatedAtUtc = utcNow;
        }

        public void MarkReady(
            string processedObjectKey,
            string publicUrl,
            DateTime utcNow)
        {
            Guard.Utc(utcNow, nameof(utcNow));

            ProcessedObjectKey = Guard.RequiredText(
                processedObjectKey,
                nameof(processedObjectKey),
                ObjectKeyMaxLength);

            PublicUrl = Guard.RequiredText(
                publicUrl,
                nameof(publicUrl),
                PublicUrlMaxLength);

            FailureReason = null;
            Status = ImageProcessingStatus.Ready;
            UpdatedAtUtc = utcNow;
        }

        public void MarkRejected(string reason, DateTime utcNow)
        {
            MarkUnsuccessful(
                ImageProcessingStatus.Rejected,
                reason,
                utcNow);
        }

        public void MarkFailed(string reason, DateTime utcNow)
        {
            MarkUnsuccessful(
                ImageProcessingStatus.Failed,
                reason,
                utcNow);
        }

        public void MarkRemoved(DateTime utcNow)
        {
            Guard.Utc(utcNow, nameof(utcNow));

            Status = ImageProcessingStatus.Removed;
            PublicUrl = null;
            FailureReason = null;
            UpdatedAtUtc = utcNow;
        }

        private void MarkUnsuccessful(
            ImageProcessingStatus status,
            string reason,
            DateTime utcNow)
        {
            Guard.Utc(utcNow, nameof(utcNow));

            FailureReason = Guard.RequiredText(
                reason,
                nameof(reason),
                FailureReasonMaxLength);

            Status = status;
            UpdatedAtUtc = utcNow;
        }
    }
}
