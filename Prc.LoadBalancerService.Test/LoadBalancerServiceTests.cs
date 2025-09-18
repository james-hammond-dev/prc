namespace Prc.LoadBalancerService.Test;

using System.Net;
using Moq;
using FluentAssertions;
using Prc.ServiceSelector;
using Prc.LoadBalancer.TcpLibrary;
using System.Text;

public class LoadBalancerServiceTests
{
    [Fact]
    public async Task StartService()
    {
        var serviceSelector = new FakeServiceSelector();
        var mockTcpFactory = new Mock<ITcpFactory>();
        var listener = new FakeTcpListener();
        var client = new FakeTcpClient(true);
        var serviceClient = new FakeTcpClient(true);

        mockTcpFactory.Setup(f => f.CreateListener(It.IsAny<IPAddress>(), It.IsAny<int>()))
                    .Returns(listener);
        mockTcpFactory.Setup(f => f.CreateClient())
            .Returns(serviceClient);

        listener.QueueClient(client);

        serviceSelector.SingleService = new BackendService { HostName = "a" };

        var config = new LoadBalancerConfig
        {
            BackendServices = new List<BackendService>
            {
                new() { HostName = "a", Port = 8081 },
                new() { HostName = "b", Port = 8082 }
            },
            ListenAddress = "0.0.0.0",
            ListenPort = 8080
        };


        var sut = new LoadBalancerService(serviceSelector, mockTcpFactory.Object, config);

        using var cts = new CancellationTokenSource();
        cts.CancelAfter(100);

        await sut.StartAsync(cts.Token);
        await Task.Delay(100);

        serviceClient.ConnectCalled.Should().BeTrue();
    }

    [Fact]
    public async Task CopyData()
    {
        var testData = Encoding.UTF8.GetBytes("Hello");
        var source = new MemoryStream(testData);
        var destination = new MemoryStream();
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(1000);

        await LoadBalancerService.CopyDataAsync(source, destination, cts.Token);

        destination.ToArray().Should().Equal(testData);
    }
}
