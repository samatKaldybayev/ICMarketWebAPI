using ICMarketWebAPI.Application.Blockchains.Commands.SyncBlockchain;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ICMarketWebAPI.Application.UnitTests.Common.Behaviours;

public class LoggingBehaviourTests
{
    private readonly Mock<ILogger<SyncBlockchainCommand>> _logger;

    public LoggingBehaviourTests()
    {
        _logger = new Mock<ILogger<SyncBlockchainCommand>>();
    }

    [Fact]
    public async Task Should_log_request_once()
    {
        var behaviour = new LoggingBehaviour<SyncBlockchainCommand>(_logger.Object);

        var command = new SyncBlockchainCommand(
            Domain.Enums.BlockchainNetwork.EthMain);

        await behaviour.Process(command, CancellationToken.None);

        _logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) =>
                    v.ToString()!.Contains(nameof(SyncBlockchainCommand))),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
