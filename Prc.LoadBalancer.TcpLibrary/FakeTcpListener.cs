namespace Prc.LoadBalancer.TcpLibrary;

using System.Threading.Tasks;

public class FakeTcpListener : ITcpListener
{
    private readonly Queue<ITcpClient> _clientQueue = new();
    public bool Active { get; private set; }
    public bool StartCalled { get; private set; }
    public bool StopCalled { get; private set; }

    public void QueueClient(ITcpClient client)
    {
        _clientQueue.Enqueue(client);
    }

    public void Start()
    {
        StartCalled = true;
        Active = true;
    }

    public void Stop()
    {
        StopCalled = true;
        Active = false;
    }

    public Task<ITcpClient> AcceptTcpClientAsync()
    {
        if (_clientQueue.Count > 0)
        {
            return Task.FromResult(_clientQueue.Dequeue());
        }

        return Task.FromResult<ITcpClient>(new FakeTcpClient());
    }
}



