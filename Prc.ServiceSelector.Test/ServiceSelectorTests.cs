namespace Prc.ServiceSelector.Test;

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
            new BackendService{HostName = "a"}
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
            new BackendService{HostName = "a"},
            new BackendService{HostName = "b"},
        };

        var sut = new ServiceSelector(services);

        _ = sut.GetNextService();

        var result = sut.GetNextService();
        result?.HostName.Should().Be("b");
    }


    [Fact]
    public void RoundRobinIndexResets()
    {
        var services = new List<BackendService>
        {
            new BackendService{HostName = "a"},
            new BackendService{HostName = "b"},
        };

        var sut = new ServiceSelector(services);

        _ = sut.GetNextService();
        _ = sut.GetNextService();

        var result = sut.GetNextService();
        result?.HostName.Should().Be("a");
    }

}
