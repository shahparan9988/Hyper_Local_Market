using HyperLocalMarket.Domain.Categories;
using HyperLocalMarket.Domain.Products;
using HyperLocalMarket.Domain.Stores;
using HyperLocalMarket.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Infrastructure.Persistence.Configurations
{
    public sealed class CategoryProposalConfiguration : IEntityTypeConfiguration<CategoryProposal>
    {
        public void Configure(
        EntityTypeBuilder<CategoryProposal> builder)
        {
            builder.ToTable("CategoryProposals");

            builder.HasKey(proposal => proposal.Id);

            builder.Property(proposal => proposal.Id)
                .ValueGeneratedNever();

            builder.Property(proposal => proposal.RequestedByUserId)
                .IsRequired();

            builder.Property(proposal => proposal.StoreId)
                .IsRequired();

            builder.Property(proposal => proposal.ProductId)
                .IsRequired();

            builder.Property(proposal => proposal.SuggestedParentCategoryId)
                .IsRequired(false);

            builder.Property(proposal => proposal.ProposedName)
                .HasMaxLength(CategoryProposal.ProposedNameMaxLength)
                .IsRequired();

            builder.Property(proposal => proposal.Description)
                .HasMaxLength(CategoryProposal.DescriptionMaxLength);

            builder.Property(proposal => proposal.Status)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(proposal => proposal.ResolvedCategoryId)
                .IsRequired(false);

            builder.Property(proposal => proposal.ResolutionNote)
                .HasMaxLength(CategoryProposal.ResolutionNoteMaxLength);

            builder.Property(proposal => proposal.CreatedAtUtc)
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            builder.Property(proposal => proposal.ResolvedAtUtc)
                .HasColumnType("timestamp with time zone");

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(proposal => proposal.RequestedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Store>()
                .WithMany()
                .HasForeignKey(proposal => proposal.StoreId)
                .OnDelete(DeleteBehavior.Restrict);

            /*
             * One product may have multiple proposals over its lifetime.
             */
            builder.HasOne<Product>()
                .WithMany()
                .HasForeignKey(proposal => proposal.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Category>()
                .WithMany()
                .HasForeignKey(proposal =>
                    proposal.SuggestedParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Category>()
                .WithMany()
                .HasForeignKey(proposal =>
                    proposal.ResolvedCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(proposal => new
            {
                proposal.ProductId,
                proposal.Status
            })
                .HasDatabaseName(
                    "IX_CategoryProposals_ProductId_Status");

            builder.HasIndex(proposal => new
            {
                proposal.StoreId,
                proposal.Status
            })
                .HasDatabaseName(
                    "IX_CategoryProposals_StoreId_Status");

            builder.HasIndex(proposal => new
            {
                proposal.Status,
                proposal.CreatedAtUtc
            })
                .HasDatabaseName(
                    "IX_CategoryProposals_Status_CreatedAtUtc");
        }
    }
}
