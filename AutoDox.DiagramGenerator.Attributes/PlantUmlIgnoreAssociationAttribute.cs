using System;

namespace AutoDox.DiagramGenerator.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class PlantUmlIgnoreAssociationAttribute : Attribute
    { }
}