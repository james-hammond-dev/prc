namespace Prc.LoadBalancer.Host;

using Microsoft.Extensions.Hosting;
using Prc.ServiceSelector;
using Prc.LoadBalancerService;
using Microsoft.Extensions.Options;
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
                new ServiceSelector(sp.GetRequiredService<LoadBalancerConfig>()));

        builder.Services.AddHostedService<LoadBalancerService>();
        var host = builder.Build();
        host.Run();
    }
}
