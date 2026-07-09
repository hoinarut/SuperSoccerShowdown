using System.Text.Json.Serialization;

namespace MyApp.Infrastructure.ExternalServices.Models;

public class StarWarsPeopleCount
{
    [JsonPropertyName("count")]
    public int Count { get; set; }
}