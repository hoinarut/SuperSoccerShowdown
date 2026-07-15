using System.Text.Json.Serialization;

namespace MyApp.Infrastructure.ExternalServices.Models;

public class PokemonPerson
{
    [JsonPropertyName("height")]
    public double Height { get; set; }

    [JsonPropertyName("weight")]
    public double Weight { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}