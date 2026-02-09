using ICMarketWebAPI.Application.Common.Interfaces;
using ICMarketWebAPI.Domain.Enums;

namespace ICMarketWebAPI.Infrastructure.Services;
public class BlockCypherClient : IBlockCypherClient
{
    private readonly HttpClient _http;

    public BlockCypherClient(HttpClient http) => _http = http;

    public async Task<string> GetNetworkJsonAsync(BlockchainNetwork network, CancellationToken ct)
    {
        var url = network switch
        {
            BlockchainNetwork.EthMain => "https://api.blockcypher.com/v1/eth/main",
            BlockchainNetwork.DashMain => "https://api.blockcypher.com/v1/dash/main",
            BlockchainNetwork.BtcMain => "https://api.blockcypher.com/v1/btc/main",
            BlockchainNetwork.BtcTest3 => "https://api.blockcypher.com/v1/btc/test3",
            BlockchainNetwork.LtcMain => "https://api.blockcypher.com/v1/ltc/main",
            _ => throw new ArgumentOutOfRangeException(nameof(network), network, null)
        };

        using var req = new HttpRequestMessage(HttpMethod.Get, url);
        using var resp = await _http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct);

        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadAsStringAsync(ct);
    }
}
