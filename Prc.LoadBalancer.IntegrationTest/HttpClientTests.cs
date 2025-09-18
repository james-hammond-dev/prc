namespace Prc.LoadBalancer.IntegrationTest;

using FluentAssertions;

public class HttpClientTests : TestBase
{
    public static HttpClient GetHttpClient()
    {
        return new HttpClient();
    }

    //re-using single client, some degree of connection re-use
    //until connection is overwhelmed / recycled
    //this might be machine/OS dependent !
    [Theory]
    [InlineData(10, 0)]
    [InlineData(100, 0)]
    [InlineData(500, 202)]
    [InlineData(600, 297)]
    public async Task ReuseOfSingleConnectionInHttpClient(int totalRequests, int expectedServer2Responses)
    {
        var loadBalancerPort = GetAvailablePort();

        var s1 = StartLocalService(8081, "s1-response");
        var s2 = StartLocalService(8082, "s2-response");

        using var process = StartLoadBalancerHost(loadBalancerPort);

        await Task.Delay(2000);

        var responses = new List<string>();

        //using the same client across requests
        //
        var client = new HttpClient();

        try
        {
            for (int i = 0; i < totalRequests; i++)
            {
                responses.Add(await client.GetStringAsync($"http://localhost:{loadBalancerPort}/"));
            }

            ValidateLoadBalancerSplitUneven(responses, totalRequests, expectedServer2Responses);
        }
        finally
        {
            s1.Close();
            s2.Close();
        }

        static void ValidateLoadBalancerSplitUneven(List<string> responses, int totalRequests, int expectedServer2Responses)
        {
            responses.Count().Should().Be(totalRequests);

            responses.Where(x => x.Contains("s1-response"))
                        .Count()
                        .Should().BeGreaterThan(totalRequests / 2);
            responses.Where(x => x.Contains("s2-response"))
                        .Count()
                        .Should().Be(expectedServer2Responses);

        }
    }
}


