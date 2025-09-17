namespace Prc.HealthCheckService.Test;
using Prc.LoadBalancer.TcpLibrary;
using Prc.ServiceSelector;

public class FakeServiceHealthChecker : IServiceHealthChecker
{
    public Task<ServiceHealth> CheckServiceHealthAsync(BackendService service, CancellationToken cancellationToken, ITcpFactory tcpFactory)
    {
        throw new NotImplementedException();
    }
}


