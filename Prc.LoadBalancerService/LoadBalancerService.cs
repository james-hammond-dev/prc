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

    public override Task StopAsync(CancellationToken token)
    {
        listener?.Stop();
        return base.StopAsync(token);
    }

    public override string? ToString()
    {
        return base.ToString();
    }

    protected override async Task ExecuteAsync(CancellationToken token)
    {
        Console.WriteLine("********* Executing ***********");
        listener = tcpFactory.CreateListener(IPAddress.Parse("0.0.0.0"), 8080);
        listener.Start();

        while (!token.IsCancellationRequested)
        {
            try
            {
                var client = await listener.AcceptTcpClientAsync();
                _ = Task.Run(() => HandleClientRequestAsync(client, token), token);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error accepting client: {ex.Message}");
                break;
            }
        }
    }

    private async Task HandleClientRequestAsync(ITcpClient client, CancellationToken token)
    {
        try
        {
            var service = serviceSelector.GetNextService();

            if (service == null)
            {
                Console.WriteLine("********* No backend services available ***********");
                return;
            }

            Console.WriteLine($"********* Service {service.HostName}:{service.Port} ***********");
            using var serviceClient = tcpFactory.CreateClient();
            await serviceClient.ConnectAsync(service.HostName, service.Port);
            Console.WriteLine($"********* Connected to {service.HostName}:{service.Port} ***********");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling client request: {ex.Message}");
        }
        finally
        {
            client?.Dispose();
        }
    }
}

