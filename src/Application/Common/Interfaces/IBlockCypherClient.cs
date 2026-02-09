using ICMarketWebAPI.Domain.Enums;

namespace ICMarketWebAPI.Application.Common.Interfaces;
public interface IBlockCypherClient
{
    Task<string> GetNetworkJsonAsync(BlockchainNetwork network, CancellationToken ct);
}
