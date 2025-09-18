
namespace Prc.ServiceSelector;

public interface IServiceSelector
{
    BackendService? GetNextService();

    List<BackendService> GetServices();

    bool SetServiceHealth(BackendService service);

    List<BackendService> Services { get; }
}

