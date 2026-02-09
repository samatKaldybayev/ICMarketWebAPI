using FluentAssertions;
using ICMarketWebAPI.Application.Blockchains.Commands.SyncBlockchain;
using ICMarketWebAPI.Application.Common.Interfaces;
using ICMarketWebAPI.Domain.Enums;
using ICMarketWebAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace ICMarketWebAPI.Application.UnitTests.Blockchains;

public class SyncBlockchainCommandHandlerTests
{
    private static ApplicationDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task Saves_snapshot_and_returns_dto()
    {
        // Arrange
        await using var db = CreateDb();
        IApplicationDbContext ctx = db;

        var client = new Mock<IBlockCypherClient>(MockBehavior.Strict);
        client.Setup(x => x.GetNetworkJsonAsync(BlockchainNetwork.EthMain, It.IsAny<CancellationToken>()))
              .ReturnsAsync("{\"height\":123}");

        var handler = new SyncBlockchainCommandHandler(ctx, client.Object);

        // Act
        var dto = await handler.Handle(
            new SyncBlockchainCommand(BlockchainNetwork.EthMain),
            CancellationToken.None);

        // Assert: client called once
        client.Verify(x => x.GetNetworkJsonAsync(BlockchainNetwork.EthMain, It.IsAny<CancellationToken>()), Times.Once);

        // Assert: DB has 1 row
        var saved = await db.BlockchainSnapshots.SingleAsync();
        saved.Network.Should().Be(BlockchainNetwork.EthMain);
        saved.RawJson.Should().Contain("123");
        saved.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        // Assert: returned dto matches saved entity
        dto.Id.Should().Be(saved.Id);
        dto.Network.Should().Be(saved.Network);
        dto.RawJson.Should().Be(saved.RawJson);
        dto.CreatedAt.Should().Be(saved.CreatedAt);
    }

    [Fact]
    public async Task Passes_cancellation_token_to_client()
    {
        // Arrange
        await using var db = CreateDb();
        IApplicationDbContext ctx = db;

        using var cts = new CancellationTokenSource();

        var client = new Mock<IBlockCypherClient>(MockBehavior.Strict);
        client.Setup(x => x.GetNetworkJsonAsync(BlockchainNetwork.BtcMain, cts.Token))
              .ReturnsAsync("{\"ok\":true}");

        var handler = new SyncBlockchainCommandHandler(ctx, client.Object);

        // Act
        await handler.Handle(new SyncBlockchainCommand(BlockchainNetwork.BtcMain), cts.Token);

        // Assert
        client.Verify(x => x.GetNetworkJsonAsync(BlockchainNetwork.BtcMain, cts.Token), Times.Once);
    }

    [Fact]
    public async Task When_client_throws_does_not_save_anything()
    {
        // Arrange
        await using var db = CreateDb();
        IApplicationDbContext ctx = db;

        var client = new Mock<IBlockCypherClient>(MockBehavior.Strict);
        client.Setup(x => x.GetNetworkJsonAsync(It.IsAny<BlockchainNetwork>(), It.IsAny<CancellationToken>()))
              .ThrowsAsync(new HttpRequestException("BlockCypher down"));

        var handler = new SyncBlockchainCommandHandler(ctx, client.Object);

        // Act
        var act = async () => await handler.Handle(
            new SyncBlockchainCommand(BlockchainNetwork.LtcMain),
            CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<HttpRequestException>()
            .WithMessage("*BlockCypher down*");

        db.BlockchainSnapshots.Should().BeEmpty();
    }
}
