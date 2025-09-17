namespace Prc.LoadBalancer.TcpLibrary;

using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

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

