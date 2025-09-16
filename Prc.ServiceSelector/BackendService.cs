
namespace Prc.ServiceSelector;

public class BackendService
{
    public string HostName { get; set; } = string.Empty;
    public int Port { get; set; }
}
public class LoadBalancerConfig
{
    //public int ListenPort { get; set; } = 8080;
    //public string ListenAddress { get; set; } = "0.0.0.0";
    public List<BackendService> BackendServices { get; set; } = new();
}
