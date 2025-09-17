namespace Prc.LoadBalancer.TcpLibrary;

using System.Threading.Tasks;

public interface ITcpListener
{
    void Start();
    void Stop();
    Task<ITcpClient> AcceptTcpClientAsync();
    bool Active { get; }
}




