using System.Reflection;
using System.Text.Json;

namespace WavedeckLabs.JsonApiSerializer;

/// <summary>
/// Serializes objects into JSON:API format.
/// </summary>
public class JsonApiSerializer
{
    private readonly string _version;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly JsonApiSerializerConfig _config;

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonApiSerializer"/> class.
    /// </summary>
    /// <param name="version">The JSON:API version to use in the serialized output.</param>
    /// <param name="config">Optional configuration settings for serialization.</param>
    public JsonApiSerializer(string version = "1.1", JsonApiSerializerConfig? config = null)
    {
        _version = version;
        _config = config ?? new JsonApiSerializerConfig();
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = _config.PropertyNamingPolicy
        };
    }
    
    /// <summary>
    /// Serializes a single object to a JSON:API formatted string.
    /// </summary>
    /// <param name="obj">The object to serialize.</param>
    /// <param name="type">The resulting resource type</param>
    /// <returns>A JSON string representing the object in JSON:API format.</returns>
    public string Serialize<T>(T obj, string? type = null)
    {
        var responseObject = PrepareSingleObject(obj, type);
        return JsonSerializer.Serialize(responseObject, _jsonSerializerOptions);
    }

    /// <summary>
    /// Serializes a collection of objects to a JSON:API formatted string.
    /// </summary>
    /// <param name="objects">The collection of objects to serialize.</param>
    /// <param name="type">The resulting resource type</param>
    /// <returns>A JSON string representing a collection of objects in JSON:API format.</returns>
    public string SerializeCollection<T>(IEnumerable<T> objects, string? type = null)
    {
        var responseObject = PrepareCollection(objects, type);
        return JsonSerializer.Serialize(responseObject, _jsonSerializerOptions);
    }

    /// <summary>
    /// Prepares a single object for JSON:API serialization.
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize.</typeparam>
    /// <param name="obj">The object to serialize. Can be null.</param>
    /// <param name="type">The resulting resource type</param>
    /// <returns>An anonymous object structured according to the JSON:API specification.</returns>
    private object PrepareSingleObject<T>(T obj, string? type = null)
    {
        if (obj == null)
        {
            return new
            {
                jsonapi = new { version = _version },
                data = (object)null!
            };
        }

        var mappedObject = MapObjectToResource(obj, type);
        return new
        {
            jsonapi = new { version = _version },
            data = mappedObject
        };
    }

    /// <summary>
    /// Prepares a collection of objects for JSON:API serialization.
    /// </summary>
    /// <typeparam name="T">The type of objects in the collection to serialize.</typeparam>
    /// <param name="objects">The collection of objects to serialize.</param>
    /// <param name="type">The resulting resource type</param>
    /// <returns>An anonymous object structured according to the JSON:API specification.</returns>
    private object PrepareCollection<T>(IEnumerable<T> objects, string? type = null)
    {
        var enumerable = objects.ToList();
        if (enumerable.Count == 0)
        {
            return new
            {
                jsonapi = new { version = _version },
                data = new List<object>()
            };
        }
        
        var data = enumerable.Select(o => MapObjectToResource(o, type)).ToList();
        return new
        {
            jsonapi = new { version = _version },
            data
        };
    }

    /// <summary>
    /// Maps an object to a JSON:API resource object structure.
    /// </summary>
    /// <typeparam name="T">The type of the object to map.</typeparam>
    /// <param name="obj">The object to map.</param>
    /// <param name="type">The resulting resource type.</param>
    /// <returns>A resource object structure.</returns>
    private object MapObjectToResource<T>(T obj, string? type = null)
    {
    // Cache Properties for faster access
    var objType = obj!.GetType();
    var allProperties = objType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
    var id = allProperties.FirstOrDefault(p => p.Name.Equals(_config.IdFieldName))?.GetValue(obj, null)?.ToString();


    // Set Resource Type either from the parameter or the object type name
    type ??= objType.Name.ToLower();

    // Retrieve all attributes properties but exclude the ID
    var properties = allProperties.Where(p => !p.Name.Equals(_config.IdFieldName)).ToList();

    // Construct the attributes dictionary
    // Note: Dictionary key names need to be converted before adding them to the dictionary
    // because the JSONSerializer Naming Policy is otherwise not applied to them
    var attributes = properties.ToDictionary(
        p => _config.PropertyNamingPolicy.ConvertName(p.Name),
        p => p.GetValue(obj));

    return new { id, type, attributes };
    }
}
