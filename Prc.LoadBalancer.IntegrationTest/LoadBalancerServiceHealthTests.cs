
namespace Prc.LoadBalancer.IntegrationTest;

using FluentAssertions;

public class LoadBalancerServiceHealthTests : TestBase
{
    [Fact]
    public async Task HealthChecker_Only_Routes_Requests_To_Healthy_Services()
    {
        var loadBalancerPort = GetAvailablePort();

        var s1 = StartLocalService(8081, "s1-response");
        var s2 = StartLocalService(8082, "s2-response");

        using var process = StartLoadBalancerHost(loadBalancerPort);

        await Task.Delay(2000);

        try
        {
            var responses = new List<string>();

            using var httpClient1 = new HttpClient();
            responses.Add(await httpClient1.GetStringAsync($"http://localhost:{loadBalancerPort}/"));

            using var httpClient2 = new HttpClient();
            responses.Add(await httpClient2.GetStringAsync($"http://localhost:{loadBalancerPort}/"));

            // each request is routed to unique server so responses are unique
            responses.Should().OnlyHaveUniqueItems();

            responses.Clear();

            //loose a service, should be flagged as unhealthy
            s2.Close();

            responses.Add(await httpClient1.GetStringAsync($"http://localhost:{loadBalancerPort}/"));
            responses.Add(await httpClient2.GetStringAsync($"http://localhost:{loadBalancerPort}/"));

            //so now only service s1 can respond
            responses.Count().Should().Be(2);
            responses.Should().OnlyContain(x => x == "Backend 8081: s1-response");
        }
        finally
        {
            process?.Kill();
            s1.Close();
            s2?.Close();
        }
    }

    [Fact]
    public async Task Three_Services_One_Becomes_Unhealthy()
    {
        var loadBalancerPort = GetAvailablePort();

        var s1 = StartLocalService(8081, "s1-response");
        var s2 = StartLocalService(8082, "s2-response");
        var s3 = StartLocalService(8083, "s3-response");

        using var process = StartLoadBalancerHost(loadBalancerPort);

        await Task.Delay(2000);

        try
        {
            var responses = new List<string>();

            using var httpClient1 = new HttpClient();
            responses.Add(await httpClient1.GetStringAsync($"http://localhost:{loadBalancerPort}/"));

            using var httpClient2 = new HttpClient();
            responses.Add(await httpClient2.GetStringAsync($"http://localhost:{loadBalancerPort}/"));

            using var httpClient3 = new HttpClient();
            responses.Add(await httpClient3.GetStringAsync($"http://localhost:{loadBalancerPort}/"));

            //LoadBalancer can route each request individually
            responses.Should().OnlyHaveUniqueItems();

            responses.Clear();

            // We loose one service
            s2.Close();

            responses.Add(await httpClient1.GetStringAsync($"http://localhost:{loadBalancerPort}/"));
            responses.Add(await httpClient2.GetStringAsync($"http://localhost:{loadBalancerPort}/"));
            responses.Add(await httpClient3.GetStringAsync($"http://localhost:{loadBalancerPort}/"));

            responses.Should().NotContain(x => x == "Backend 8082: s2-response");
        }
        finally
        {
            process?.Kill();
            s1.Close();
            s2?.Close();
            s3.Close();
        }
    }
}

