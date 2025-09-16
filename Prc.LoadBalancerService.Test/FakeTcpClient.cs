namespace Prc.LoadBalancerService.Test;

using System.Net;

public class FakeTcpClient : ITcpClient
{
    private readonly MemoryStream stream = new();
    public bool Connected { get; set; } = true;
    public EndPoint? RemoteEndPoint { get; set; } = new IPEndPoint(IPAddress.Loopback, 12345);
    public bool ConnectCalled { get; private set; }
    public bool CloseCalled { get; private set; }

    public Task ConnectAsync(string hostname, int port)
    {
        ConnectCalled = true;
        return Task.CompletedTask;
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

