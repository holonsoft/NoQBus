namespace holonsoft.NoQBus.Abstractions.Attributes;

/// <summary>
/// This attribute denies a message to be sent to a remote endpoint. Helpful to force message to be local.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class DenyRemotingAttribute : Attribute
{
}
