using holonsoft.Utils;
using System.Collections;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace holonsoft.NoQBus.Serialization.PolymorphyHelper;
internal class JsonSerializerPolymorphyConverter : JsonConverterFactory
{
  private readonly ConcurrentDictionary<Type, JsonConverter> _converterCache = new();

  //only way to stop endless recursion because we want to use the same serializer to serialize our object as well...
  public Type LastConvertedType { get; set; }

  public override bool CanConvert(Type typeToConvert)
  {
    var result
      = typeToConvert != LastConvertedType
        && !typeToConvert.IsArray
        && !typeToConvert.IsEnum
        && !typeToConvert.IsValueType
        && !typeToConvert.Namespace.StartsWith(nameof(System), StringComparison.OrdinalIgnoreCase)
        && !typeToConvert.IsSubclassOf(typeof(IEnumerable));

    if (!result)
    {
      LastConvertedType = null;
    }

    return result;
  }

  public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    => _converterCache
      .GetOrAdd(typeToConvert, x =>
        (JsonConverter) Activator.CreateInstance(typeof(JsonSerializerPolymorphyConverter<>).MakeGenericType(x), this));
}

internal class JsonSerializerPolymorphyConverter<T> : JsonConverter<T>
{
  private const string _typeNameField = "$typeName";
  private const string _typeValueField = "$typeValue";

  private readonly JsonSerializerPolymorphyConverter _parentFactory;
  //if we dont cache the options we have a performance loss ~1000x
  private JsonSerializerOptions _optionsClone;
  public JsonSerializerPolymorphyConverter(JsonSerializerPolymorphyConverter parentFactory)
    => _parentFactory = parentFactory;

  public override bool HandleNull => false;

  public override bool CanConvert(Type typeToConvert)
    => _parentFactory.CanConvert(typeToConvert);

  public override T Read(
     ref Utf8JsonReader reader,
     Type typeToConvert,
     JsonSerializerOptions options)
  {
    if (reader.TokenType == JsonTokenType.Null)
    {
      return default;
    }

    if (reader.TokenType != JsonTokenType.StartObject)
    {
      throw new JsonException();
    }

    if (!reader.Read() || reader.TokenType != JsonTokenType.PropertyName)
    {
      throw new JsonException();
    }

    if (reader.GetString() != _typeNameField)
    {
      throw new JsonException();
    }

    if (!reader.Read())
    {
      throw new JsonException();
    }

    var typeName = reader.GetString();

    if (string.IsNullOrWhiteSpace(typeName))
    {
      throw new JsonException();
    }

    if (!ReflectionUtils.AllNonAbstractTypes.TryGetValue(typeName, out var type))
    {
      throw new JsonException();
    }

    if (!reader.Read() || reader.GetString() != _typeValueField)
    {
      throw new JsonException();
    }
    if (!reader.Read() || reader.TokenType != JsonTokenType.StartObject)
    {
      throw new JsonException();
    }

    _parentFactory.LastConvertedType = type;
    var result = (T) JsonSerializer.Deserialize(ref reader, type, _optionsClone ??= new JsonSerializerOptions(options));

    if (!reader.Read() || reader.TokenType != JsonTokenType.EndObject)
    {
      throw new JsonException();
    }

    return result;
  }

  public override void Write(
      Utf8JsonWriter writer,
      T value,
      JsonSerializerOptions options)
  {
    writer.WriteStartObject();

    writer.WriteString(_typeNameField, value.GetType().FullName);
    writer.WritePropertyName(_typeValueField);

    _parentFactory.LastConvertedType = value.GetType();
    JsonSerializer.Serialize(writer, value, value.GetType(), _optionsClone ??= new JsonSerializerOptions(options));

    writer.WriteEndObject();
  }
}
