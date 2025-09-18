namespace Prc.HealthCheckService.Test;

using FluentAssertions;
using Moq;
using Prc.LoadBalancer.TcpLibrary;
using Prc.ServiceSelector;

public class ServiceHealthCheckerTests
{
    [Fact]
    public async Task WhenServiceHealthIsOk()
    {
        var service = new BackendService { };

        using var cts = new CancellationTokenSource();
        cts.CancelAfter(100);

        var client = new FakeTcpClient(true);

        var mockTcpFactory = new Mock<ITcpFactory>();

        mockTcpFactory.Setup(f => f.CreateClient())
                    .Returns(client);

        var sut = new ServiceHealthChecker();

        var result = await sut.CheckServiceHealthAsync(service, cts.Token, mockTcpFactory.Object);

        result.IsHealthy.Should().BeTrue();
    }

    [Fact]
    public async Task WhenServiceHealthIsBad()
    {
        var service = new BackendService { };

        using var cts = new CancellationTokenSource();
        cts.CancelAfter(100);

        var client = new FakeTcpClient(false);

        var mockTcpFactory = new Mock<ITcpFactory>();

        mockTcpFactory.Setup(f => f.CreateClient())
                    .Returns(client);

        var sut = new ServiceHealthChecker();

        var result = await sut.CheckServiceHealthAsync(service, cts.Token, mockTcpFactory.Object);

        result.IsHealthy.Should().BeFalse();
    }
}


