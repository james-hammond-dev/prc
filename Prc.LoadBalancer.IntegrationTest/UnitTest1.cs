namespace Prc.LoadBalancer.IntegrationTest;

using System.Diagnostics;
using FluentAssertions;

public class LoadBalancerSimpleTests : TestBase
{
    //TODO: based on the host config for now, look at injecting config on host startup

    [Fact]
    public async Task BackendServicesRunning()
    {
        var s1 = StartLocalService(8081);
        var s2 = StartLocalService(8082);
        var s3 = StartLocalService(8083);

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
            using var httpClient1 = new HttpClient();
            var response1 = await httpClient1.GetStringAsync("http://localhost:8080/");

            using var httpClient2 = new HttpClient();
            var response2 = await httpClient2.GetStringAsync("http://localhost:8080/");

            using var httpClient3 = new HttpClient();
            var response3 = await httpClient3.GetStringAsync("http://localhost:8080/");

            using var httpClient4 = new HttpClient();
            var response4 = await httpClient4.GetStringAsync("http://localhost:8080/");


            Console.WriteLine(response1);
            Console.WriteLine(response2);
            Console.WriteLine(response3);
            Console.WriteLine(response4);

            //response1.Should().Contain("8081");
            //response2.Should().Contain("8082");
            //response2.Should().Contain("8083");

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
