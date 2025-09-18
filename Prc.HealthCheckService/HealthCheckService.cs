namespace Prc.HealthCheckService;

using Microsoft.Extensions.Hosting;
using Prc.ServiceSelector;
using Prc.LoadBalancer.TcpLibrary;

public class HealthCheckService : BackgroundService
{
    private readonly IServiceSelector serviceSelector;
    private readonly IServiceHealthChecker healthChecker;
    private readonly ITcpFactory tcpFactory;

    public HealthCheckService(IServiceSelector serviceSelector,
            IServiceHealthChecker healthChecker,
            ITcpFactory tcpFactory)
    {
        this.serviceSelector = serviceSelector;
        this.healthChecker = healthChecker;
        this.tcpFactory = tcpFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var interval = 3;

        Console.WriteLine($"Health check service started with {interval}s interval");

        while (!stoppingToken.IsCancellationRequested)
        {
            await CheckAllServicesHealth(stoppingToken);
            await Task.Delay(TimeSpan.FromSeconds(interval), stoppingToken);
        }
    }

    private async Task CheckAllServicesHealth(CancellationToken cancellationToken)
    {
        var services = serviceSelector.GetServices();

        var healthCheckTasks = services.Select(async service =>
        {
            try
            {
                var s = await healthChecker
                .CheckServiceHealthAsync(service, cancellationToken, tcpFactory);

                if (service.ServiceHealth != s)
                {
                    service.ServiceHealth = s;

                    serviceSelector.SetServiceHealth(service);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Health check failed for {service.HostName}:{service.Port}: {ex.Message}");
            }
        });

        await Task.WhenAll(healthCheckTasks);
    }
}
