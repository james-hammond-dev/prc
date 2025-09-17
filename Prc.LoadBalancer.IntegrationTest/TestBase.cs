namespace Prc.LoadBalancer.IntegrationTest;

using System.Net;
using System.Text;

public class TestBase
{
    public HttpListener StartLocalService(int port, string serviceResponse)
    {
        var listener = new HttpListener();
        listener.Prefixes.Add($"http://localhost:{port}/");
        listener.Start();

        Task.Run(async () =>
        {
            while (listener.IsListening)
            {
                var context = await listener.GetContextAsync();
                var responseBytes = Encoding.UTF8.GetBytes($"Backend {port}: {serviceResponse}");
                context.Response.ContentLength64 = responseBytes.Length;
                await context.Response.OutputStream.WriteAsync(responseBytes);
                context.Response.Close();
            }
        });

        return listener;
    }
}

