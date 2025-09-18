namespace Prc.LoadBalancer.IntegrationTest;

using System.Diagnostics;
using FluentAssertions;

public class LoadBalancerSimpleTests : TestBase
{
    //TODO: based on the host config for now, look at injecting config on host startup
    // (listening port is dynamic for now, ok so far to just use the servics configured
    // // in the host project

    [Fact]
    public async Task ThreeConfiguredServices_Three_Http_Client_Requests()
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

            responses.Should().OnlyHaveUniqueItems();

        }
        finally
        {
            process?.Kill();
            s1.Close();
            s2.Close();
            s3.Close();
        }
    }
}
