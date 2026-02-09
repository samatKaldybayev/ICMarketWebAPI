using ICMarketWebAPI.Application.Blockchains.Commands.SyncBlockchain;
using ICMarketWebAPI.Application.Blockchains.DTOs;
using ICMarketWebAPI.Application.Blockchains.Queries.GetBlockchainHistory;
using ICMarketWebAPI.Application.Common.Exceptions;
using ICMarketWebAPI.Domain.Enums;

namespace ICMarketWebAPI.Web.Endpoints;

public class Blockchains : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(Sync, "{chain}/sync")
            .MapGet(History, "{chain}/history");
    }

    public Task<BlockchainSnapshotDto> Sync(ISender sender, string chain, CancellationToken ct)
        => sender.Send(new SyncBlockchainCommand(Parse(chain)), ct);

    public Task<IReadOnlyList<BlockchainSnapshotDto>> History(
        ISender sender,
        string chain,
        int take = 100,
        CancellationToken ct = default)
        => sender.Send(new GetBlockchainHistoryQuery(Parse(chain), take), ct);

    private static BlockchainNetwork Parse(string chain) =>
        chain.Trim().ToLowerInvariant() switch
        {
            "eth" or "ethmain" => BlockchainNetwork.EthMain,
            "dash" or "dashmain" => BlockchainNetwork.DashMain,
            "btc" or "btcmain" => BlockchainNetwork.BtcMain,
            "btctest3" or "test3" => BlockchainNetwork.BtcTest3,
            "ltc" or "ltcmain" => BlockchainNetwork.LtcMain,
            _ => throw new ValidationException(
                $"Unknown chain '{chain}'. Use: eth, dash, btc, btctest3, ltc.")
        };
}
