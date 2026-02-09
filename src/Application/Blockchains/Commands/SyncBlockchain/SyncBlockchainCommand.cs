using ICMarketWebAPI.Application.Blockchains.DTOs;
using ICMarketWebAPI.Domain.Enums;

namespace ICMarketWebAPI.Application.Blockchains.Commands.SyncBlockchain;
public record SyncBlockchainCommand(BlockchainNetwork Network) : IRequest<BlockchainSnapshotDto>;
