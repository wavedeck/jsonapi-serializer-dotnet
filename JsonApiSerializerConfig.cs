using System.Text.Json;

namespace WavedeckLabs.JsonApiSerializer;

public class JsonApiSerializerConfig
{
    public string IdFieldName { get; set; } = "Id";
    
    public JsonNamingPolicy PropertyNamingPolicy { get; set; } = JsonNamingPolicy.CamelCase;
}