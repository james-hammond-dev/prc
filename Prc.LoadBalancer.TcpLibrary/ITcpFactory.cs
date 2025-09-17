namespace Prc.LoadBalancer.TcpLibrary;

using System.Net;

public interface ITcpFactory
{
    ITcpListener CreateListener(IPAddress address, int port);
    ITcpClient CreateClient();
}



