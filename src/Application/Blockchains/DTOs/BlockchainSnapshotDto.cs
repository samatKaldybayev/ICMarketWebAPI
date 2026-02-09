using ICMarketWebAPI.Domain.Enums;

namespace ICMarketWebAPI.Application.Blockchains.DTOs;
public record BlockchainSnapshotDto(
    int Id,
    BlockchainNetwork Network,
    DateTime CreatedAt,
    string RawJson
);
