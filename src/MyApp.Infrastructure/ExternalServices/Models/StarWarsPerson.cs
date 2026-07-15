using System.Text.Json.Serialization;

namespace MyApp.Infrastructure.ExternalServices.Models;

public class StarWarsPerson
{
    [JsonPropertyName("height")]
    public string Height { get; set; } = string.Empty;

    [JsonPropertyName("mass")]
    public string Mass { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}