namespace Suni.Suni.NikoSharp.Data.Types;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false)]
public class ExposedPropertyAttribute : Attribute
{
    public string Name { get; }
    public ExposedPropertyAttribute(string name) => Name = name;
}
