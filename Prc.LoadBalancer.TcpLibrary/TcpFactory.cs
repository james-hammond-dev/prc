namespace Prc.LoadBalancer.TcpLibrary;

using System.Net;

public class TcpFactory : ITcpFactory
{
    public ITcpListener CreateListener(IPAddress address, int port)
        => new TcpListenerWrapper(address, port);

    public ITcpClient CreateClient()
        => new TcpClientWrapper();
}



