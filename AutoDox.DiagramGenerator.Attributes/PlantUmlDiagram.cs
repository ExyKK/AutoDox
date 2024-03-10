using System;

namespace AutoDox.DiagramGenerator.Attributes
{
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Interface|AttributeTargets.Enum|AttributeTargets.Struct)]
    public class PlantUmlDiagramAttribute : Attribute
    { }
}