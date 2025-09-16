namespace Prc.LoadBalancer.Host;

using Microsoft.Extensions.Hosting;
using Prc.ServiceSelector;
using Prc.LoadBalancerService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddSingleton<IServiceSelector, ServiceSelector>();
        builder.Services.AddSingleton<ITcpFactory, TcpFactory>();
        builder.Services.AddHostedService<LoadBalancerService>();
        var host = builder.Build();
        host.Run();
    }
}
