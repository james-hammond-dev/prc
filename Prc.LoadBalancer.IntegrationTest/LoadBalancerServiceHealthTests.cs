
namespace Prc.LoadBalancer.IntegrationTest;

using System.Diagnostics;
using FluentAssertions;


public class LoadBalancerServiceHealthTests : TestBase
{
    [Fact]
    public async Task HealthChecker_Only_Routes_Requests_To_Healthy_Services()
    {
        //TODO : this delay is to allow socket teardown from previous tests. improve sln.
        await Task.Delay(5000);

        var s1 = StartLocalService(8081, "s1-response");
        var s2 = StartLocalService(8082, "s2-response");

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

            responses.Should().OnlyHaveUniqueItems();

            responses.Clear();

            s2.Close();

            responses.Add(await httpClient1.GetStringAsync("http://localhost:8080/"));
            responses.Add(await httpClient2.GetStringAsync("http://localhost:8080/"));

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

            //LoadBalancer can route each request individually
            responses.Should().OnlyHaveUniqueItems();

            responses.Clear();

            // We loose one service
            s2.Close();

            responses.Add(await httpClient1.GetStringAsync("http://localhost:8080/"));
            responses.Add(await httpClient2.GetStringAsync("http://localhost:8080/"));
            responses.Add(await httpClient3.GetStringAsync("http://localhost:8080/"));

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

