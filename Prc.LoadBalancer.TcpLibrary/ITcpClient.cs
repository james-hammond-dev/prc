namespace Prc.LoadBalancer.TcpLibrary;

using System.Net;
using System.Threading.Tasks;

public interface ITcpClient : IDisposable
{
    bool Connected { get; }
    EndPoint? RemoteEndPoint { get; }
    Task ConnectAsync(string hostname, int port);
    Stream GetStream();
    void Close();
}

