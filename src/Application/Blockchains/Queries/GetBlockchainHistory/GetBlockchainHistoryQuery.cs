using ICMarketWebAPI.Application.Blockchains.DTOs;
using ICMarketWebAPI.Domain.Enums;

namespace ICMarketWebAPI.Application.Blockchains.Queries.GetBlockchainHistory;
public record GetBlockchainHistoryQuery(BlockchainNetwork Network, int Take = 100) : IRequest<IReadOnlyList<BlockchainSnapshotDto>>;
