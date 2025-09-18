namespace Prc.LoadBalancer.IntegrationTest;

using System.Diagnostics;
using FluentAssertions;

public class LoadBalancerLoadTests : TestBase
{
    // start n bg services
    // use loop ? to create lots of http client instances
    // check truth, 1 http client will multiplex connections so how does this affect lb?
    //

}

public class LoadBalancerSimpleTests : TestBase
{
    //TODO: based on the host config for now, look at injecting config on host startup

    [Fact]
    public async Task ThreeConfiguredServices_Three_Http_Client_Requests()
    {
        //TODO : this delay is to allow socket teardown from previous tests. improve sln.
        await Task.Delay(5000);

        var s1 = StartLocalService(8081, "s1-response");
        var s2 = StartLocalService(8082, "s2-response");
        var s3 = StartLocalService(8083, "s3-response");

        using var process = Process.Start(new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = "run --project ../../../../Prc.LoadBalancer.Host",
            UseShellExecute = false,
            RedirectStandardOutput = true
        });

        await Task.Delay(2000);

        try
        {
            var responses = new List<string>();

            using var httpClient1 = new HttpClient();
            responses.Add(await httpClient1.GetStringAsync("http://localhost:8080/"));

            using var httpClient2 = new HttpClient();
            responses.Add(await httpClient2.GetStringAsync("http://localhost:8080/"));

            using var httpClient3 = new HttpClient();
            responses.Add(await httpClient3.GetStringAsync("http://localhost:8080/"));

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
