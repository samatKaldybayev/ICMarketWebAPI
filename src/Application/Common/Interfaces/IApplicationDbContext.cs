using ICMarketWebAPI.Domain.Entities;

namespace ICMarketWebAPI.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<BlockchainSnapshot> BlockchainSnapshots { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
