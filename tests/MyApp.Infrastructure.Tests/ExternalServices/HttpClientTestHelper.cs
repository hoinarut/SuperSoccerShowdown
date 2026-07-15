using System.Net;
using System.Text;

namespace MyApp.Infrastructure.Tests.ExternalServices;

internal sealed class MockHttpMessageHandler : HttpMessageHandler
{
    private readonly Func<HttpRequestMessage, HttpResponseMessage> _handler;

    public MockHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> handler)
    {
        _handler = handler;
    }

    public List<HttpRequestMessage> Requests { get; } = [];

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        Requests.Add(request);
        return Task.FromResult(_handler(request));
    }

    public static HttpResponseMessage JsonResponse(string json, HttpStatusCode statusCode = HttpStatusCode.OK) =>
        new(statusCode)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
}

internal static class HttpClientTestHelper
{
    public static HttpClient CreateClient(MockHttpMessageHandler handler, string baseAddress = "https://example.com/") =>
        new(handler, disposeHandler: true)
        {
            BaseAddress = new Uri(baseAddress)
        };
}
