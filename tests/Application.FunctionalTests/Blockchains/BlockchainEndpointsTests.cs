using System.Net.Http.Json;
using ICMarketWebAPI.Application.Blockchains.DTOs;

public class BlockchainEndpointsTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public BlockchainEndpointsTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Sync_endpoint_returns_snapshot()
    {
        var response = await _client.PostAsync("/api/blockchains/eth/sync", null);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        var dto = await response.Content.ReadFromJsonAsync<BlockchainSnapshotDto>();
        dto.Should().NotBeNull();
        dto!.Network.ToString().ToLower().Should().Contain("eth");
    }
}
