namespace Prc.LoadBalancer.TcpLibrary;

using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

public class FakeTcpClient : ITcpClient
{
    public FakeTcpClient()
    {

    }

    public FakeTcpClient(bool connectSuccess)
    {
        ConnectCalled = connectSuccess;
    }

    private readonly MemoryStream stream = new();
    public bool Connected { get; set; } = true;
    public EndPoint? RemoteEndPoint { get; set; } = new IPEndPoint(IPAddress.Loopback, 12345);
    public bool ConnectCalled { get; private set; }
    public bool CloseCalled { get; private set; }

    public Task ConnectAsync(string hostname, int port)
    {
        if (ConnectCalled == true)
            return Task.CompletedTask;

        //TODO : good enough for our purposes ?
        return Task.FromException(new SocketException());
    }

    public Stream GetStream() => stream;

    public void Close()
    {
        CloseCalled = true;
    }

    public void Dispose()
    {
        stream?.Dispose();
        GC.SuppressFinalize(this);
    }
}


