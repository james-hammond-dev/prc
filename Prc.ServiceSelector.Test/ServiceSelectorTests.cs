namespace Prc.ServiceSelector.Test;

using Microsoft.Extensions.Options;
using FluentAssertions;

public class ServiceSelectorTests
{
    [Fact]
    public void NoBackendServicesAvailable()
    {
        var services = new List<BackendService> { };

        var sut = new ServiceSelector(services);

        var result = sut.GetNextService();

        //TOO: consider a different impl e.g. response object
        result.Should().BeNull();
    }

    [Fact]
    public void SingleBackendService()
    {
        var services = new List<BackendService>
        {
            new (){HostName = "a", Port=8081, ServiceHealth = new ServiceHealth(true,DateTime.UtcNow,"ok")}
        };

        var sut = new ServiceSelector(services);

        var result = sut.GetNextService();

        result?.HostName.Should().Be("a");
    }

    [Fact]
    public void TwoAvailableServices()
    {
        var services = new List<BackendService>
        {
            new ()
        {HostName = "a",Port=8081, ServiceHealth = new ServiceHealth(true,DateTime.UtcNow,"ok")},
            new ()
        {HostName = "b",Port=8082, ServiceHealth = new ServiceHealth(true,DateTime.UtcNow,"ok")},
        };

        var sut = new ServiceSelector(services);

        _ = sut.GetNextService();

        var result = sut.GetNextService();
        result?.HostName.Should().Be("b");
    }

    [Fact]
    public void TwoAvailableServices_One_Is_UnHealthy()
    {
        var services = new List<BackendService>
        {
            new (){HostName = "a",Port = 8081, ServiceHealth = new ServiceHealth(true,DateTime.UtcNow,"ok")},
            new (){HostName = "b", Port = 8082, ServiceHealth = new ServiceHealth(false,DateTime.UtcNow,"bad")}
        };

        var sut = new ServiceSelector(services);

        var result = sut.GetNextService();
        result?.HostName.Should().Be("a");
    }

    [Fact]
    public void RoundRobinIndexResets()
    {
        var services = new List<BackendService>
        {
            new (){HostName = "a",Port = 8081, ServiceHealth = new ServiceHealth(true,DateTime.UtcNow,"ok")},
            new (){HostName = "b",Port = 8082, ServiceHealth = new ServiceHealth(true,DateTime.UtcNow,"ok")},

        };

        var sut = new ServiceSelector(services);

        _ = sut.GetNextService();
        _ = sut.GetNextService();

        var result = sut.GetNextService();
        result?.HostName.Should().Be("a");
    }

    [Fact]
    public void ConfigureServicesFromConfig()
    {
        var config = new LoadBalancerConfig
        {
            BackendServices = new List<BackendService>
            {
                new() { HostName = "a", Port = 8081, ServiceHealth = new ServiceHealth(true,DateTime.UtcNow,"ok")},
                new() { HostName = "b", Port = 8082, ServiceHealth = new ServiceHealth(true,DateTime.UtcNow,"ok")},
            }
        };

        var options = Options.Create(config);

        var sut = new ServiceSelector(options.Value);

        var result = sut.GetNextService();
        result?.HostName.Should().Be("a");
    }

    [Fact]
    public void SetServiceHealth()
    {
        var services = new List<BackendService>
        {
            new (){HostName = "a", Port = 8081 },
            new (){HostName = "b", Port = 8082 },
        };

        var sut = new ServiceSelector(services);

        var unhealthyService = new BackendService
        {
            HostName = "a",
            Port = 8081,
            ServiceHealth = new ServiceHealth(false, DateTime.UtcNow, "my-info")
        };

        var updated = sut.SetServiceHealth(unhealthyService);

        updated.Should().BeTrue();

        var x = sut
            .Services
            .SingleOrDefault(s =>
                    s.HostName == unhealthyService.HostName
                    && s.Port == unhealthyService.Port);

        x!.ServiceHealth!.IsHealthy.Should().BeFalse();
    }

    //TODO: think about behaviour when there's no matching service.

}
