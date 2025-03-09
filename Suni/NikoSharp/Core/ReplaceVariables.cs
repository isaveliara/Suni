using System.Text.RegularExpressions;
using Suni.Suni.NikoSharp.Data.Types;
namespace Suni.Suni.NikoSharp.Core;

public partial class NptSystem
{
    internal string ReplaceVariables(string line)
    {
        return Regex.Replace(line, @"\${(\w+)}", match => {
            string varName = match.Groups[1].Value;
            if (ContextData.Variables.Any(v => v.ContainsKey(varName))){
                SType value = ContextData.Variables.First(v => v.ContainsKey(varName))[varName];
                if (value.Type == STypes.Function)
                    return value.ToString();
                return value?.ToString() ?? "nil";
            }
            else{
                ContextData.Debugs.Add($"Warning: Variable '{varName}' not found. Returning nil.");
                return "nil";
            }
        });
    }
}