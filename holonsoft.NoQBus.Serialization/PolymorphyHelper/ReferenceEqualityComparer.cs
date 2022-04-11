using System.Collections;

namespace holonsoft.NoQBus.Serialization.PolymorphyHelper;
internal sealed class ReferenceEqualityComparer : IEqualityComparer<object>, IEqualityComparer
{
  private ReferenceEqualityComparer() { }

  public static ReferenceEqualityComparer Instance { get; }
    = new ReferenceEqualityComparer();

  public new bool Equals(object x, object y)
    => ReferenceEquals(x, y);

  public int GetHashCode(object obj)
    => obj.GetHashCode();
}
