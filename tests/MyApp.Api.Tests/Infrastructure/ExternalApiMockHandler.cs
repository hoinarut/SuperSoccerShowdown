using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace MyApp.Api.Tests.Infrastructure;

internal sealed partial class ExternalApiMockHandler : HttpMessageHandler
{
    [GeneratedRegex(@"/people/(\d+)$")]
    private static partial Regex StarWarsPersonRegex();

    [GeneratedRegex(@"/pokemon/(\d+)$")]
    private static partial Regex PokemonPersonRegex();

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var url = request.RequestUri!.ToString();

        if (url.EndsWith("/people", StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(Json("""{"count": 50}"""));
        }

        var starWarsMatch = StarWarsPersonRegex().Match(url);
        if (starWarsMatch.Success)
        {
            var id = starWarsMatch.Groups[1].Value;
            return Task.FromResult(Json(
                $$"""{"name": "SW-{{id}}", "mass": "80", "height": "180"}"""));
        }

        if (url.EndsWith("/pokemon", StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(Json("""{"count": 50}"""));
        }

        var pokemonMatch = PokemonPersonRegex().Match(url);
        if (pokemonMatch.Success)
        {
            var id = pokemonMatch.Groups[1].Value;
            return Task.FromResult(Json(
                $$"""{"name": "pokemon-{{id}}", "weight": 600, "height": 18}"""));
        }

        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
    }

    private static HttpResponseMessage Json(string json) =>
        new(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
}
