using HyperLocalMarket.Domain.Categories.Events;
using HyperLocalMarket.Domain.common;
using HyperLocalMarket.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Domain.Categories
{
    public sealed class CategoryProposal : AggregateRoot
    {
        public const int ProposedNameMaxLength = 150;
        public const int DescriptionMaxLength = 1_000;
        public const int ResolutionNoteMaxLength = 1_000;

        private CategoryProposal()
        {
        }

        private CategoryProposal(
            Guid requestedByUserId,
            Guid storeId,
            Guid productId,
            Guid? suggestedParentCategoryId,
            string proposedName,
            string? description,
            DateTime utcNow)
        {
            Guard.NotEmpty(requestedByUserId, nameof(requestedByUserId));
            Guard.NotEmpty(storeId, nameof(storeId));
            Guard.NotEmpty(productId, nameof(productId));
            ValidateNullableId(
                suggestedParentCategoryId,
                nameof(suggestedParentCategoryId));
            Guard.Utc(utcNow, nameof(utcNow));

            RequestedByUserId = requestedByUserId;
            StoreId = storeId;
            ProductId = productId;
            SuggestedParentCategoryId = suggestedParentCategoryId;
            ProposedName = Guard.RequiredText(
                proposedName,
                nameof(proposedName),
                ProposedNameMaxLength);
            Description = Guard.OptionalText(
                description,
                nameof(description),
                DescriptionMaxLength);
            Status = CategoryProposalStatus.Pending;
            CreatedAtUtc = utcNow;
        }

        public Guid RequestedByUserId { get; private set; }

        public Guid StoreId { get; private set; }

        public Guid ProductId { get; private set; }

        public Guid? SuggestedParentCategoryId { get; private set; }

        public string ProposedName { get; private set; } = default!;

        public string? Description { get; private set; }

        public CategoryProposalStatus Status { get; private set; }

        public Guid? ResolvedCategoryId { get; private set; }

        public string? ResolutionNote { get; private set; }

        public DateTime CreatedAtUtc { get; private set; }

        public DateTime? ResolvedAtUtc { get; private set; }

        public static CategoryProposal Create(
            Guid requestedByUserId,
            Guid storeId,
            Guid productId,
            Guid? suggestedParentCategoryId,
            string proposedName,
            string? description,
            DateTime utcNow)
        {
            var proposal = new CategoryProposal(
                requestedByUserId,
                storeId,
                productId,
                suggestedParentCategoryId,
                proposedName,
                description,
                utcNow);

            proposal.AddDomainEvent(
                new CategoryProposalCreatedDomainEvent(
                    proposal.Id,
                    productId,
                    storeId,
                    proposal.ProposedName,
                    utcNow));

            return proposal;
        }

        public void ApproveAsNewCategory(
            Guid categoryId,
            string? resolutionNote,
            DateTime utcNow)
        {
            Resolve(
                CategoryProposalStatus.Approved,
                categoryId,
                resolutionNote,
                utcNow);
        }

        public void MapToExistingCategory(
            Guid categoryId,
            string? resolutionNote,
            DateTime utcNow)
        {
            Resolve(
                CategoryProposalStatus.MappedToExisting,
                categoryId,
                resolutionNote,
                utcNow);
        }

        public void Reject(
            string resolutionNote,
            DateTime utcNow)
        {
            var normalizedNote = Guard.RequiredText(
                resolutionNote,
                nameof(resolutionNote),
                ResolutionNoteMaxLength);

            Resolve(
                CategoryProposalStatus.Rejected,
                null,
                normalizedNote,
                utcNow);
        }

        public void Cancel(DateTime utcNow)
        {
            Resolve(
                CategoryProposalStatus.Cancelled,
                null,
                null,
                utcNow);
        }

        private void Resolve(
            CategoryProposalStatus newStatus,
            Guid? resolvedCategoryId,
            string? resolutionNote,
            DateTime utcNow)
        {
            EnsurePending();
            ValidateNullableId(resolvedCategoryId, nameof(resolvedCategoryId));
            Guard.Utc(utcNow, nameof(utcNow));

            if (newStatus is CategoryProposalStatus.Approved
                or CategoryProposalStatus.MappedToExisting)
            {
                if (!resolvedCategoryId.HasValue)
                {
                    throw new DomainException(
                        "An approved or mapped proposal requires a resolved category.");
                }
            }
            else if (resolvedCategoryId.HasValue)
            {
                throw new DomainException(
                    "A rejected or cancelled proposal cannot resolve to a category.");
            }

            Status = newStatus;
            ResolvedCategoryId = resolvedCategoryId;
            ResolutionNote = Guard.OptionalText(
                resolutionNote,
                nameof(resolutionNote),
                ResolutionNoteMaxLength);
            ResolvedAtUtc = utcNow;

            AddDomainEvent(
                new CategoryProposalResolvedDomainEvent(
                    Id,
                    ProductId,
                    newStatus,
                    resolvedCategoryId,
                    utcNow));
        }

        private void EnsurePending()
        {
            if (Status != CategoryProposalStatus.Pending)
            {
                throw new DomainException(
                    "Only a pending category proposal can be resolved.");
            }
        }

        private static void ValidateNullableId(
            Guid? value,
            string parameterName)
        {
            if (value == Guid.Empty)
            {
                throw new DomainException(
                    $"{parameterName} cannot be an empty identifier.");
            }
        }
    }
}
