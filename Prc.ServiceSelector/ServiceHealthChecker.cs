namespace Prc.ServiceSelector;

using Prc.LoadBalancer.TcpLibrary;

public class ServiceHealthChecker
{
    public async Task<ServiceHealth> CheckServerHealthAsync(BackendService service,
            CancellationToken cancellationToken,
            ITcpFactory tcpFactory)
    {
        using var client = tcpFactory.CreateClient();
        var connectTask = client.ConnectAsync(service.HostName, service.Port);
        var timeoutTask = Task.Delay(5000, cancellationToken);

        var completedTask = await Task.WhenAny(connectTask, timeoutTask);

        if (!connectTask.IsFaulted)
        {
            return new ServiceHealth(true, DateTime.UtcNow, "looks-good");
        }

        return new ServiceHealth(false, DateTime.UtcNow, "?");
    }
}

