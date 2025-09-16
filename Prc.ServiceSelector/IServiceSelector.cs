
namespace Prc.ServiceSelector;

public interface IServiceSelector
{
    BackendService? GetNextService();
}

