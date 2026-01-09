using HyperLocalMarket.Application.Common.Interfaces.Persistence;
using HyperLocalMarket.Domain.common;
using HyperLocalMarket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public DbSet<Store> Stores => Set<Store>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public new Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    
    }
}
