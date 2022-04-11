using holonsoft.NoQBus.Abstractions.Contracts;
using holonsoft.NoQBus.Serialization.PolymorphyHelper;
using System.Text;
using System.Text.Json;

namespace holonsoft.NoQBus.Serialization;
public class MessageSerializer : IMessageSerializer
{
  private readonly Encoding _encoding = Encoding.UTF8;

  private JsonSerializerOptions CreateSerializerOptions()
  {
    JsonSerializerOptions options = new()
    {
      ReferenceHandler = new RootedPreserveReferenceHandler(), //for the converter - thats why have all the time to create this JsonSerializerOptions new!
      WriteIndented = true
    };

    options.Converters.Add(new JsonSerializerPolymorphyConverter());

    return options;
  }

  public object Deserialize(Type type, byte[] serialized)
    => JsonSerializer.Deserialize(_encoding.GetString(serialized), type, CreateSerializerOptions());

  public byte[] Serialize(object toSerialize)
    => _encoding.GetBytes(JsonSerializer.Serialize(toSerialize, toSerialize.GetType(), CreateSerializerOptions()));


}
