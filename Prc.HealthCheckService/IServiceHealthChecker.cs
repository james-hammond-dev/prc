namespace Prc.HealthCheckService;
using Prc.ServiceSelector;
using Prc.LoadBalancer.TcpLibrary;

public interface IServiceHealthChecker
{
    Task<ServiceHealth> CheckServiceHealthAsync(BackendService service,
            CancellationToken cancellationToken,
            ITcpFactory tcpFactory);
}

