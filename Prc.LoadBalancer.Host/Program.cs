namespace Prc.LoadBalancer.Host;

using Microsoft.Extensions.Hosting;
using Prc.ServiceSelector;
using Prc.LoadBalancerService;
using Microsoft.Extensions.Options;
using Prc.LoadBalancer.TcpLibrary;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        builder.Services
            .Configure<LoadBalancerConfig>(
                builder.Configuration.GetSection("LoadBalancer"));
        builder.Services.AddSingleton<ITcpFactory, TcpFactory>();

        builder.Services.AddSingleton<LoadBalancerConfig>(sp =>
                sp.GetRequiredService<IOptions<LoadBalancerConfig>>().Value);

        builder.Services.AddSingleton<IServiceSelector, ServiceSelector>(sp =>
                new ServiceSelector(sp.GetRequiredService<LoadBalancerConfig>()
                            .BackendServices.OrderBy(x => x.Port).ToList()));

        builder.Services.AddSingleton<IServiceHealthChecker, ServiceHealthChecker>();
        builder.Services.AddHostedService<LoadBalancerService>();
        builder.Services.AddHostedService<HealthCheckService>();
        var host = builder.Build();
        host.Run();
    }
}
