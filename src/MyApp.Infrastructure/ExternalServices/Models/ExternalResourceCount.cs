using System.Text.Json.Serialization;

namespace MyApp.Infrastructure.ExternalServices.Models;

public class ExternalResourceCount
{
    [JsonPropertyName("count")]
    public int Count { get; set; }
}