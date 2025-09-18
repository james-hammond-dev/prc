namespace Prc.LoadBalancer.IntegrationTest;

using FluentAssertions;

public class LoadBalancerLoadTests : TestBase
{
    // start n bg services
    // use loop ? to create lots of http client instances
    // check truth, 1 http client will multiplex connections so how does this affect lb?
    //

    [Fact]
    public async Task SingleThread_ClientRotation()
    {
        var loadBalancerPort = GetAvailablePort();

        var s1 = StartLocalService(8081, "s1-response");
        var s2 = StartLocalService(8082, "s2-response");

        using var process = StartLoadBalancerHost(loadBalancerPort);

        await Task.Delay(2000);

        var responses = new List<string>();

        try
        {
            const int totalRequests = 1000;

            for (int i = 0; i < totalRequests; i++)
            {
                using var client = new HttpClient();
                responses.Add(await client.GetStringAsync($"http://localhost:{loadBalancerPort}/"));

                client.Dispose();
            }

            ValidateLoadBalancerSplit(responses, totalRequests);
        }
        finally
        {
            s1.Close();
            s2.Close();
        }

        static void ValidateLoadBalancerSplit(List<string> responses, int totalRequests)
        {
            responses.Count().Should().Be(totalRequests);

            responses.Where(x => x.Contains("s1-response"))
                        .Count()
                .Should().Be(totalRequests / 2);
            responses.Where(x => x.Contains("s2-response"))
                        .Count()
                .Should().Be(totalRequests / 2);
        }
    }
}

