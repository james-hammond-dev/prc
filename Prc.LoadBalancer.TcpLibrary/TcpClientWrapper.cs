namespace Prc.LoadBalancer.TcpLibrary;

using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

public class TcpClientWrapper : ITcpClient
{
    private readonly TcpClient _client;

    public TcpClientWrapper(TcpClient client)
    {
        _client = client;
    }

    public TcpClientWrapper() : this(new TcpClient()) { }

    public bool Connected => _client.Connected;
    public EndPoint? RemoteEndPoint => _client.Client.RemoteEndPoint;

    public async Task ConnectAsync(string hostname, int port)
    {
        await _client.ConnectAsync(hostname, port);
    }

    public Stream GetStream() => _client.GetStream();
    public void Close() => _client.Close();

    public void Dispose()
    {
        _client?.Dispose();
        GC.SuppressFinalize(this);
    }
}

