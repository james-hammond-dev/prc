namespace Prc.HealthCheckService.Test;
using Prc.LoadBalancer.TcpLibrary;
using Prc.ServiceSelector;

public class FakeServiceHealthChecker : IServiceHealthChecker
{
    public bool SetServiceAsHealthy { get; set; }

    public Task<ServiceHealth> CheckServiceHealthAsync(BackendService service, CancellationToken cancellationToken, ITcpFactory tcpFactory)
    {
        if (SetServiceAsHealthy)
        {
            return Task.Run(() => new ServiceHealth(true, DateTime.UtcNow, "Healthy"));
        }

        return Task.Run(() => new ServiceHealth(false, DateTime.UtcNow, "UnHealthy"));
    }
}


