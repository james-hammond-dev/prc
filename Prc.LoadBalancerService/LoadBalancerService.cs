namespace Prc.LoadBalancerService;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Prc.ServiceSelector;

public class LoadBalancerService : BackgroundService
{
    private readonly IServiceSelector serviceSelector;
    private readonly ITcpFactory tcpFactory;
    private ITcpListener? listener;

    public override Task? ExecuteTask => base.ExecuteTask;

    public LoadBalancerService(IServiceSelector serviceSelector, ITcpFactory tcpFactory)
    {
        this.serviceSelector = serviceSelector;
        this.tcpFactory = tcpFactory;
    }

    public override void Dispose()
    {
        base.Dispose();
    }

    public override bool Equals(object? obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override async Task StartAsync(CancellationToken token)
    {
        Console.WriteLine("********* Starting ***********");
        await ExecuteAsync(token);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        return base.StopAsync(cancellationToken);
    }

    public override string? ToString()
    {
        return base.ToString();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("********* Executing ***********");
        listener = tcpFactory.CreateListener(IPAddress.Parse("0.0.0.0"), 8080);
        listener.Start();

        var client = await listener.AcceptTcpClientAsync();
        _ = Task.Run(() => HandleClientRequestAsync(client, stoppingToken), stoppingToken);

    }

    private async Task HandleClientRequestAsync(ITcpClient client, CancellationToken token)
    {
        var service = serviceSelector.GetNextService();

        Console.WriteLine($"********* Service {service.HostName} ***********");
        using var serviceClient = tcpFactory.CreateClient();
        await serviceClient.ConnectAsync(service.HostName, 8080);
    }
}

