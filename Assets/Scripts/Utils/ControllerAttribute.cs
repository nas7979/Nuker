using System;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
public sealed class ControllerAttribute : Attribute
{
    public ControllerAttribute() { }
}