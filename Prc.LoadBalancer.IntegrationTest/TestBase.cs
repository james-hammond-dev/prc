
[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Prc.LoadBalancer.IntegrationTest;

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Diagnostics;

public class TestBase
{
    public HttpListener StartLocalService(int port, string serviceResponse)
    {
        var listener = new HttpListener();
        listener.Prefixes.Add($"http://localhost:{port}/");
        listener.Start();

        Task.Run(async () =>
        {
            while (listener.IsListening)
            {
                var context = await listener.GetContextAsync();
                var responseBytes = Encoding.UTF8.GetBytes($"Backend {port}: {serviceResponse}");
                context.Response.ContentLength64 = responseBytes.Length;
                await context.Response.OutputStream.WriteAsync(responseBytes);
                context.Response.Close();
            }
        });

        return listener;
    }

    public static int GetAvailablePort()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }

    //start the host using the port we tried to get dynamically,
    //reduces test flakiness / errors around socket allocations
    public Process StartLoadBalancerHost(int loadBalancerPort)
    {
        var process = Process.Start(new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = "run --project ../../../../Prc.LoadBalancer.Host",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            Environment = {
                ["LoadBalancer__ListenPort"] = loadBalancerPort.ToString(),
                ["LoadBalancer__ListenAddress"] = "127.0.0.1"
            }
        });

        return process ?? throw new InvalidOperationException("Failed to start LoadBalancer process");
    }
}

