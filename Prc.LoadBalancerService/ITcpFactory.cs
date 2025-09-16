namespace Prc.LoadBalancerService;

using System.Net;
using System.Net.Sockets;

public interface ITcpFactory
{
    ITcpListener CreateListener(IPAddress address, int port);
    ITcpClient CreateClient();
}


public class TcpFactory : ITcpFactory
{
    public ITcpListener CreateListener(IPAddress address, int port)
        => new TcpListenerWrapper(address, port);

    public ITcpClient CreateClient()
        => new TcpClientWrapper();
}

public class TcpListenerWrapper : ITcpListener
{
    public readonly TcpListener listener;
    private bool isActive = false;

    public TcpListenerWrapper(IPAddress address, int port)
    {
        listener = new TcpListener(address, port);
    }

    public bool Active => isActive;

    public void Start()
    {
        listener.Start();
        isActive = true;
    }
    public void Stop()
    {
        listener.Stop();
        isActive = false;
    }

    public async Task<ITcpClient> AcceptTcpClientAsync()
    {
        var client = await listener.AcceptTcpClientAsync();
        return new TcpClientWrapper(client);
    }
}

