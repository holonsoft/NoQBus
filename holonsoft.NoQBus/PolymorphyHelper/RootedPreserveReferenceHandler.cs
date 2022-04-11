using System.Text.Json.Serialization;

namespace holonsoft.NoQBus.PolymorphyHelper;
internal class RootedPreserveReferenceHandler : ReferenceHandler
{
  private ReferenceResolver _rootedResolver;

  public override ReferenceResolver CreateResolver()
     => _rootedResolver ??= new RootedPreserveReferenceResolver();
}
