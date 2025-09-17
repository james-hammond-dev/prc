namespace Prc.LoadBalancerService.Test;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Prc.LoadBalancer.TcpLibrary;
using Prc.ServiceSelector;

public class FakeServiceHealthChecker : IServiceHealthChecker
{
    public Task<ServiceHealth> CheckServiceHealthAsync(BackendService service, CancellationToken cancellationToken, ITcpFactory tcpFactory)
    {
        throw new NotImplementedException();
    }
}

public class FakeServiceSelector : IServiceSelector
{
    public BackendService? SingleService { get; set; }

    private List<BackendService>? services;

    public FakeServiceSelector()
    {

    }

    public FakeServiceSelector(List<BackendService> services)
    {
        this.services = services;
    }

    public BackendService? GetNextService()
    {
        return SingleService;
    }

    public List<BackendService> GetServices()
    {
        return services;
    }

    public bool SetServiceHealth(BackendService service)
    {
        var x = services.SingleOrDefault(s =>
                    s.HostName == service.HostName
                    && s.Port == service.Port);

        if (x == null) return false;

        x.ServiceHealth = service.ServiceHealth;

        return true;

    }
}

