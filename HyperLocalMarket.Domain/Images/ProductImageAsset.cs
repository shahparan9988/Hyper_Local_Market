using HyperLocalMarket.Domain.common;
using HyperLocalMarket.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Domain.Images
{
    public sealed class ProductImageAsset : AggregateRoot
    {
        public const long MaximumBytes = 10 * 1024 * 1024;
        private ProductImageAsset() { }
        private ProductImageAsset(
        Guid id,
        Guid storeId,
        Guid userId,
        string originalObjectKey,
        string contentType,
        long size,
        DateTime now)
        : base(id)
        {
            StoreId = storeId;
            UserId = userId;
            OriginalObjectKey = originalObjectKey;
            ContentType = contentType;
            ExpectedSizeBytes = size;

            Status = ImageProcessingStatus.PendingUpload;

            CreatedAtUtc = now;
            UpdatedAtUtc = now;
        }
        public static ProductImageAsset Create(
            Guid id,
            Guid storeId,
            Guid userId,
            string originalObjectKey,
            string contentType,
            long size,
            DateTime now)
        {
            Guard.NotEmpty(id, nameof(id));
            Guard.NotEmpty(storeId, nameof(storeId));
            Guard.NotEmpty(userId, nameof(userId));
            Guard.Utc(now, nameof(now));

            ProductImageRules.ValidateUpload(
                contentType,
                size);

            originalObjectKey = Guard.RequiredText(
                originalObjectKey,
                nameof(originalObjectKey),
                1000);

            return new ProductImageAsset(
                id,
                storeId,
                userId,
                originalObjectKey,
                contentType,
                size,
                now);
        }
        //public ProductImageAsset(
        //    Guid storeId,
        //    Guid userId,
        //    string contentType,
        //    long size,
        //    DateTime now)
        //{
        //    Guard.NotEmpty(storeId, nameof(storeId));
        //    Guard.NotEmpty(userId, nameof(userId));
        //    Guard.Utc(now, nameof(now));
        //    var extension = contentType
        //        switch {
        //            "image/jpeg" => ".jpg",
        //            "image/png" => ".png",
        //            "image/webp" => ".webp",
        //            _ => throw new DomainException("Only JPEG, PNG and WebP are allowed.")
        //        };
        //    if (size <= 0 || size > MaximumBytes)
        //        throw new DomainException("The image must be between 1 byte and 10 MB.");
        //    StoreId = storeId;
        //    UserId = userId;
        //    ContentType = contentType;
        //    ExpectedSizeBytes = size;

        //    OriginalObjectKey = $"incoming/products/{storeId:N}/{Id:N}{extension}";
        //    Status = ImageProcessingStatus.PendingUpload;
        //    CreatedAtUtc = UpdatedAtUtc = now;
        //}
        public Guid StoreId { get; private set; }
        public Guid UserId { get; private set; }
        public string OriginalObjectKey { get; private set; } = "";
        public string? ProcessedObjectKey { get; private set; }
        public string ContentType { get; private set; } = "";
        public long ExpectedSizeBytes { get; private set; }
        public ImageProcessingStatus Status { get; private set; }
        public string? PublicUrl { get; private set; }
        public string? FailureReason { get; private set; }
        public int Version { get; private set; } = 1;
        public DateTime CreatedAtUtc { get; private set; }
        public DateTime UpdatedAtUtc { get; private set; }
        public DateTime? StorageCleanedAtUtc { get; private set; }
        public void MarkStorageCleaned(DateTime now) {
            StorageCleanedAtUtc = now;
            Touch(now);
        }
        public bool IsTerminal =>
            Status is ImageProcessingStatus.Ready
                or ImageProcessingStatus.Rejected
                or ImageProcessingStatus.Failed
                or ImageProcessingStatus.Removed;
        public void MarkProcessing(DateTime now) {

            if (!IsTerminal) {
                Status = ImageProcessingStatus.Processing;
                Touch(now);
            } 
        }
        public void MarkReady(string key, string url, DateTime now)
        {
            if (IsTerminal)
                return;
            ProcessedObjectKey = Guard.RequiredText(key, nameof(key), 1000);
            PublicUrl = Guard.RequiredText(url, nameof(url), 2000);
            Status = ImageProcessingStatus.Ready;
            FailureReason = null;
            Touch(now);
        }
        public void Reject(string reason, DateTime now) {
            if (!IsTerminal) {
                Status = ImageProcessingStatus.Rejected;
                FailureReason = reason[..Math.Min(reason.Length, 500)];
                Touch(now);
            } 
        }
        public void Fail(DateTime now) {
            if (!IsTerminal) {
                Status = ImageProcessingStatus.Failed;
                FailureReason = "Image processing failed after multiple attempts.";
                Touch(now);
            } 
        }
        public void Remove(DateTime now) {
            Status = ImageProcessingStatus.Removed;
            PublicUrl = null;
            Touch(now);
        }
        private void Touch(DateTime now) {
            Guard.Utc(now, nameof(now));
            UpdatedAtUtc = now;
            Version = checked(Version + 1);
        }
    }

}
