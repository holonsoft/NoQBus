using System.Text.Json;
using System.Text.Json.Serialization;

namespace holonsoft.NoQBus.PolymorphyHelper;
internal class RootedPreserveReferenceResolver : ReferenceResolver
{
  private uint _referenceCount;
  private readonly Dictionary<string, object> _referenceIdToObjectMap;
  private readonly Dictionary<object, string> _objectToReferenceIdMap;

  public RootedPreserveReferenceResolver()
  {
    _objectToReferenceIdMap = new Dictionary<object, string>(ReferenceEqualityComparer.Instance);
    _referenceIdToObjectMap = new Dictionary<string, object>();
  }

  public override void AddReference(string referenceId, object value) => _referenceIdToObjectMap[referenceId] = value;

  public override string GetReference(object value, out bool alreadyExists)
  {
    if (_objectToReferenceIdMap.TryGetValue(value, out var referenceId))
    {
      alreadyExists = true;
    }
    else
    {
      _referenceCount++;
      referenceId = _referenceCount.ToString();
      _objectToReferenceIdMap.Add(value, referenceId);
      alreadyExists = false;
    }

    return referenceId;
  }

  public override object ResolveReference(string referenceId)
  {
    if (!_referenceIdToObjectMap.TryGetValue(referenceId, out var value))
    {
      throw new JsonException();
    }

    return value;
  }
}
