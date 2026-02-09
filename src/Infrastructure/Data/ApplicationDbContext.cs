using System.Reflection;
using ICMarketWebAPI.Application.Common.Interfaces;
using ICMarketWebAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ICMarketWebAPI.Infrastructure.Data;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<BlockchainSnapshot> BlockchainSnapshots => Set<BlockchainSnapshot>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
