using Suni.Suni.NikoSharp.Data;
using Suni.Suni.NikoSharp.Data.Types;
namespace Suni.Suni.NikoSharp.Core.Evaluator;
partial class NikoSharpEvaluator
{
    private static SType AccessProperty(SType target, string property)
    {
        var type = target.GetType();
        //searches for methods marked with the 'ExposedProperty' attribute.
        var methods = type.GetMethods()
            .Where(m => m.GetCustomAttributes(typeof(ExposedPropertyAttribute), false)
                        .OfType<ExposedPropertyAttribute>()
                        .Any(attr => attr.Name == property))
            .ToList();

        if (methods.Count > 0){
            var method = methods[0];
            return (SType)method.Invoke(target, null);
        }

        //searches for properties marked with the 'ExposedProperty' attribute.
        var properties = type.GetProperties()
            .Where(p => p.GetCustomAttributes(typeof(ExposedPropertyAttribute), false)
                        .OfType<ExposedPropertyAttribute>()
                        .Any(attr => attr.Name == property))
            .ToList();

        if (properties.Count > 0){
            var propertyInfo = properties[0];
            return (SType)propertyInfo.GetValue(target);
        }

        return new NikosError(Diagnostics.UnlistedProperty, $"type 'STypes.{target.Type}' doesn't have the property '{property}'");
    }
}