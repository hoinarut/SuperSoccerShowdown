using System.Text.Json.Serialization;

namespace MyApp.Infrastructure.ExternalServices.Models;

public class StarWarsPerson
{
    [JsonPropertyName("height")]
    public decimal Height { get; set; }

    [JsonPropertyName("mass")]
    public decimal Mass { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }
}