namespace Prc.HealthCheckService.Test;

using FluentAssertions;
using Moq;
using Prc.LoadBalancer.TcpLibrary;
using Prc.ServiceSelector;

public class HealthCheckServiceTests
{
    [Fact]
    public async Task StartupHealthCheckServiceTest()
    {
        var services = new List<BackendService>
        {
            new (){HostName = "a"},
            new (){HostName = "b"},
        };

        var serviceSelector = new ServiceSelector(services);

        var healthChecker = new ServiceHealthChecker();

        var mockTcpFactory = new Mock<ITcpFactory>();
        var client = new FakeTcpClient(true);

        mockTcpFactory.Setup(f => f.CreateClient())
                .Returns(client);

        var sut = new HealthCheckService(serviceSelector, healthChecker, mockTcpFactory.Object);

        using var cts = new CancellationTokenSource();

        await sut.StartAsync(cts.Token);

        await sut.StopAsync(cts.Token);

        await Task.Delay(200);

        client.ConnectCalled.Should().BeTrue();
    }

    //TODO : if we expand the Fake services we could dynamically add/remove services
    // to simulate health failure/check conditions

}

