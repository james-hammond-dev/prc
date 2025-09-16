namespace Prc.LoadBalancerService.Test;
using Prc.ServiceSelector;

public class FakeServiceSelector : IServiceSelector
{
    public BackendService? SingleService { get; set; }

    public BackendService? GetNextService()
    {
        return SingleService;
    }
}

