namespace Prc.LoadBalancerService.Test;

public class FakeTcpListener : ITcpListener
{
    private readonly Queue<ITcpClient> queue = new();
    public bool Active { get; private set; }
    public bool StartCalled { get; private set; }
    public bool StopCalled { get; private set; }

    public void QueueClient(ITcpClient client)
    {
        queue.Enqueue(client);
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
        if (queue.Count > 0)
        {
            return Task.FromResult(queue.Dequeue());
        }

        return Task.FromResult<ITcpClient>(new FakeTcpClient());
    }
}

