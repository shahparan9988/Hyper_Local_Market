using HyperLocalMarket.Infrastructure.Persistence.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Infrastructure.Persistence.Configurations
{
    public sealed class OutboxMessageConfiguration
       : IEntityTypeConfiguration<OutboxMessage>
    {
        public void Configure(EntityTypeBuilder<OutboxMessage> builder)
        {
            builder.ToTable("outbox_messages");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Type).IsRequired();

            builder.Property(x => x.Content).IsRequired();

            builder.Property(x => x.OccurredAtUtc).IsRequired();

            builder.Property(x => x.RetryCount).IsRequired();

            builder.HasIndex(x => new
            {
                x.ProcessedAtUtc,
                x.RetryCount
            });
        }
    }
}
