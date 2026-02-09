using ICMarketWebAPI.Domain.Entities;

namespace ICMarketWebAPI.Application.Blockchains.DTOs;

public class BlockchainSnapshotProfile : Profile
{
    public BlockchainSnapshotProfile()
    {
        CreateMap<BlockchainSnapshot, BlockchainSnapshotDto>();
    }
}
