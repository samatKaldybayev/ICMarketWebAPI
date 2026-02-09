using AutoMapper;
using FluentAssertions;
using ICMarketWebAPI.Application.Blockchains.DTOs;
using ICMarketWebAPI.Application.Blockchains.Queries.GetBlockchainHistory;
using ICMarketWebAPI.Domain.Entities;
using ICMarketWebAPI.Domain.Enums;
using ICMarketWebAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ICMarketWebAPI.Application.UnitTests.Blockchains;

public class GetBlockchainHistoryTests
{

    private static IMapper CreateMapper()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddMaps(typeof(BlockchainSnapshotDto).Assembly);
        });

        config.AssertConfigurationIsValid();
        return config.CreateMapper();
    }

    private static ApplicationDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task Returns_only_requested_network_and_orders_by_createdAt_desc()
    {
        // Arrange
        await using var db = CreateDb();

        var older = new BlockchainSnapshot
        {
            Network = BlockchainNetwork.EthMain,
            CreatedAt = DateTime.UtcNow.AddMinutes(-10),
            RawJson = "{ \"block\": 1 }"
        };

        var newer = new BlockchainSnapshot
        {
            Network = BlockchainNetwork.EthMain,
            CreatedAt = DateTime.UtcNow,
            RawJson = "{ \"block\": 2 }"
        };

        db.BlockchainSnapshots.AddRange(
            older,
            newer,
            new BlockchainSnapshot // other network
            {
                Network = BlockchainNetwork.BtcMain,
                CreatedAt = DateTime.UtcNow.AddMinutes(-5),
                RawJson = "{ \"block\": 999 }"
            });

        await db.SaveChangesAsync();

        var mapper = CreateMapper();
        var handler = new GetBlockchainHistoryQueryHandler(db, mapper);
        var query = new GetBlockchainHistoryQuery(BlockchainNetwork.EthMain, Take: 10);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result.Select(x => x.Network).Distinct().Should().ContainSingle().Which.Should().Be(BlockchainNetwork.EthMain);

        result[0].CreatedAt.Should().BeAfter(result[1].CreatedAt);
        result[0].RawJson.Should().Contain("\"block\": 2");
        result[1].RawJson.Should().Contain("\"block\": 1");
    }

    [Fact]
    public async Task Applies_take_limit()
    {
        // Arrange
        await using var db = CreateDb();

        for (int i = 0; i < 5; i++)
        {
            db.BlockchainSnapshots.Add(new BlockchainSnapshot
            {
                Network = BlockchainNetwork.BtcMain,
                CreatedAt = DateTime.UtcNow.AddMinutes(-i),
                RawJson = $"{{ \"block\": {i} }}"
            });
        }

        await db.SaveChangesAsync();

        var mapper = CreateMapper();
        var handler = new GetBlockchainHistoryQueryHandler(db, mapper);

        // Act
        var result = await handler.Handle(
            new GetBlockchainHistoryQuery(BlockchainNetwork.BtcMain, Take: 3),
            CancellationToken.None);

        // Assert
        result.Should().HaveCount(3);
        result.Should().BeInDescendingOrder(x => x.CreatedAt);
    }

    [Fact]
    public async Task Returns_empty_when_no_data()
    {
        // Arrange
        await using var db = CreateDb();
        var mapper = CreateMapper();
        var handler = new GetBlockchainHistoryQueryHandler(db, mapper);

        // Act
        var result = await handler.Handle(
            new GetBlockchainHistoryQuery(BlockchainNetwork.LtcMain, Take: 100),
            CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
}
