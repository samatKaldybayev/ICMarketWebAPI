using ICMarketWebAPI.Application.Blockchains.DTOs;
using ICMarketWebAPI.Application.Common.Interfaces;
using ICMarketWebAPI.Domain.Entities;

namespace ICMarketWebAPI.Application.Blockchains.Commands.SyncBlockchain;
public class SyncBlockchainCommandHandler : IRequestHandler<SyncBlockchainCommand, BlockchainSnapshotDto>
{
    private readonly IApplicationDbContext _db;
    private readonly IBlockCypherClient _client;

    public SyncBlockchainCommandHandler(IApplicationDbContext db, IBlockCypherClient client)
    {
        _db = db;
        _client = client;
    }

    public async Task<BlockchainSnapshotDto> Handle(SyncBlockchainCommand request, CancellationToken ct)
    {
        var raw = await _client.GetNetworkJsonAsync(request.Network, ct);

        var entity = new BlockchainSnapshot
        {
            Network = request.Network,
            RawJson = raw,
            CreatedAt = DateTime.UtcNow
        };

        _db.BlockchainSnapshots.Add(entity);
        await _db.SaveChangesAsync(ct);

        return new BlockchainSnapshotDto(entity.Id, entity.Network, entity.CreatedAt, entity.RawJson);
    }
}
