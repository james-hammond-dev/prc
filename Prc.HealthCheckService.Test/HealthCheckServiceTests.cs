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
    [Fact]
    public async Task WhenAServiceIsUnhealthyUpdateTheSelector()
    {
        var services = new List<BackendService>
        {
            new (){HostName = "a",Port = 8081,ServiceHealth = new (true,DateTime.UtcNow,"")},
            new (){HostName = "b",Port = 8081,ServiceHealth = new (true,DateTime.UtcNow,"")}
        };

        var serviceSelector = new Mock<IServiceSelector>();
        serviceSelector.Setup(x => x.Services).Returns(services);
        serviceSelector.Setup(x => x.GetServices()).Returns(services);
        //when this gets called we mark the servic as unhealthy
        var healthChecker = new FakeServiceHealthChecker();
        healthChecker.SetServiceAsHealthy = false;

        var mockTcpFactory = new Mock<ITcpFactory>();
        var client = new FakeTcpClient(true);

        mockTcpFactory.Setup(f => f.CreateClient())
                .Returns(client);

        var sut = new HealthCheckService(serviceSelector.Object, healthChecker, mockTcpFactory.Object);

        using var cts = new CancellationTokenSource();

        await sut.StartAsync(cts.Token);

        await Task.Delay(6000);

        await sut.StopAsync(cts.Token);

        serviceSelector.Verify(x => x.SetServiceHealth(It.IsAny<BackendService>()), Times.AtLeastOnce);
    }
}

