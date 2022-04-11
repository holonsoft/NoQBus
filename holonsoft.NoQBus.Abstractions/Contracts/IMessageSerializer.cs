namespace holonsoft.NoQBus.Abstractions.Contracts;
public interface IMessageSerializer
{
  public byte[] Serialize(object toSerialize);
  public object Deserialize(Type type, byte[] serialized);
}
