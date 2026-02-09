using ICMarketWebAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ICMarketWebAPI.Infrastructure.Data.Configurations;
public class BlockchainSnapshotConfiguration : IEntityTypeConfiguration<BlockchainSnapshot>
{
    public void Configure(EntityTypeBuilder<BlockchainSnapshot> builder)
    {
        builder.Property(x => x.RawJson).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.Network).IsRequired();

        builder.HasIndex(x => new { x.Network, x.CreatedAt });
    }
}
