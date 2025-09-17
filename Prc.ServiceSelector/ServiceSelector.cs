using Microsoft.Extensions.Options;

namespace Prc.ServiceSelector;

public class ServiceSelector : IServiceSelector
{
    private List<BackendService> services;
    private int currentIndex;
    private readonly LoadBalancerConfig? config;

    public ServiceSelector(List<BackendService> services)
    {
        this.services = services;
        currentIndex = 0;
    }

    public ServiceSelector(LoadBalancerConfig config)
    {
        services = config.BackendServices;
        currentIndex = 0;
    }

    public BackendService? GetNextService()
    {
        if (!services.Any()) return null;

        if (currentIndex >= services.Count)
        {
            currentIndex = 0;
        }

        var service = services[currentIndex];

        currentIndex = (currentIndex + 1);

        return service;
    }
}
