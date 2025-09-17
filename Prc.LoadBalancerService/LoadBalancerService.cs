namespace Prc.LoadBalancerService;

using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Prc.ServiceSelector;
using Prc.LoadBalancer.TcpLibrary;

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

            using var serviceClient = tcpFactory.CreateClient();
            await serviceClient.ConnectAsync(service.HostName, service.Port);

            var clientStream = client.GetStream();
            var serviceStream = serviceClient.GetStream();

            var task1 = CopyDataAsync(clientStream, serviceStream, token);
            var task2 = CopyDataAsync(serviceStream, clientStream, token);

            await Task.WhenAll(task1, task2);
        }
        catch (OperationCanceledException)
        {
            // Expected
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

    public static async Task CopyDataAsync(Stream source, Stream destination, CancellationToken cancellationToken)
    {
        try
        {
            var buffer = new byte[8192];
            int bytesRead;

            while ((bytesRead = await source.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
            {
                await destination.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                await destination.FlushAsync(cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (ObjectDisposedException)
        {
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in transfer : {ex.Message}");
        }
    }
}

