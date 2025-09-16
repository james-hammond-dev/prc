namespace Prc.LoadBalancerService.Test;

using System.Net;
using Moq;
using FluentAssertions;

using Prc.ServiceSelector;

public class LoadBalancerServiceTests
{

    [Fact]
    public async Task StartService()
    {
        var serviceSelector = new FakeServiceSelector();
        var mockTcpFactory = new Mock<ITcpFactory>();
        var listener = new FakeTcpListener();
        var client = new FakeTcpClient();
        var serviceClient = new FakeTcpClient();

        mockTcpFactory.Setup(f => f.CreateListener(It.IsAny<IPAddress>(), It.IsAny<int>()))
                    .Returns(listener);
        mockTcpFactory.Setup(f => f.CreateClient())
            .Returns(serviceClient);

        listener.QueueClient(client);

        serviceSelector.SingleService = new BackendService { HostName = "a" };

        var sut = new LoadBalancerService(serviceSelector, mockTcpFactory.Object);

        using var cts = new CancellationTokenSource();
        cts.CancelAfter(100);

        await sut.StartAsync(cts.Token);
        await Task.Delay(100);

        serviceClient.ConnectCalled.Should().BeTrue();
    }
}
