using ICMarketWebAPI.Application.Blockchains.DTOs;
using ICMarketWebAPI.Application.Blockchains.Queries.GetBlockchainHistory;
using ICMarketWebAPI.Application.Common.Interfaces;

public class GetBlockchainHistoryQueryHandler
    : IRequestHandler<GetBlockchainHistoryQuery, IReadOnlyList<BlockchainSnapshotDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly IConfigurationProvider _mapper;

    public GetBlockchainHistoryQueryHandler(
        IApplicationDbContext db,
        IMapper mapper)
    {
        _db = db;
        _mapper = mapper.ConfigurationProvider;
    }

    public async Task<IReadOnlyList<BlockchainSnapshotDto>> Handle(
        GetBlockchainHistoryQuery request,
        CancellationToken ct)
    {
        var take = request.Take is < 1 or > 1000 ? 100 : request.Take;

        return await _db.BlockchainSnapshots
            .Where(x => x.Network == request.Network)
            .OrderByDescending(x => x.CreatedAt)
            .ProjectTo<BlockchainSnapshotDto>(_mapper)
            .Take(take)
            .ToListAsync(ct);
    }
}
