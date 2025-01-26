using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Suni.Suni.NptEnvironment.Data;
using System.Reflection;
using Suni.Suni.NptEnvironment.Core;

namespace Suni.Suni.NptEnvironment.Formalizer;

public partial class FormalizingScript
{
    internal static (Dictionary<string, List<string>> includes, List<Dictionary<string, NptTypes.NptType>> variables, Diagnostics) InterpretDefinitionsBlock(List<string> lines)
    {
        //(default)
        var includes = new Dictionary<string, List<string>>{
            { "std", NptSystem.MainControlerLibMethods }
        };
        var variables = new List<Dictionary<string, NptTypes.NptType>>{
            new Dictionary<string, NptTypes.NptType> { { "__version__", new NptTypes.NptType(NptTypes.Types.Str, SunClassBot.SuniV) } },
            new Dictionary<string, NptTypes.NptType> { { "__time__", new NptTypes.NptType(NptTypes.Types.Str, DateTime.Now.ToString()) } }
        };

        for (int i = 0; i < lines.Count; i++)
        {
            string currentLine = lines[i].Trim();

            //process "include" statements
            if (currentLine.StartsWith("~include")){
                var includeName = currentLine.Substring(9).Trim();
                Console.WriteLine($"{includeName} included by line: {currentLine}");

                if (!string.IsNullOrWhiteSpace(includeName) && !includes.ContainsKey(includeName)){
                    string classNameProper = char.ToUpper(includeName[0]) + includeName.Substring(1).ToLower() + "Entitie";
                    Type type = Type.GetType($"Sun.NPT.ScriptInterpreter.{classNameProper}");

                    if (type != null){
                        var libMethodsProperty = type.GetProperty("LibMethods", BindingFlags.Static | BindingFlags.Public);
                        if (libMethodsProperty != null){
                            var methods = libMethodsProperty.GetValue(null) as List<string>;
                            if (methods != null && methods.Any()){
                                includes[includeName] = methods;
                                Console.WriteLine($"Library '{includeName}' registered with methods: {string.Join(", ", methods)}");
                            }
                            else{
                                includes[includeName] = new List<string>();
                                Console.WriteLine($"Library '{includeName}' registered but no methods found.");
                            }
                        }
                        else{
                            includes[includeName] = new List<string>();
                            Console.WriteLine($"Library '{includeName}' found, but 'LibMethods' property is missing.");
                        }
                    }
                    else{
                        Console.WriteLine($"Error: Library '{includeName}' class '{classNameProper}' not found.");
                        return (null, null, Diagnostics.IncludeNotFoundException);
                    }
                }
            }

            //process "set" statements for variables
            if (currentLine.StartsWith("~set"))
            {
                var parts = currentLine.Substring(4).Trim().Split(' ', 2);
                if (parts.Length == 2)
                {
                    string variableName = parts[0].Trim();
                    Console.WriteLine($"Detected ~set for variable or function: {variableName}");

                    var fnMatch = Regex.Match(variableName, @"\[(\w+)<([^>]*)>\]");
                    if (fnMatch.Success)
                    {
                        string fnName = fnMatch.Groups[1].Value;
                        var parameters = fnMatch.Groups[2].Value
                            .Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(param => param.Trim())
                            .ToList();
                        string code = parts[1].Trim().Trim('"');

                        Console.WriteLine($"Registering function: {fnName}\nParameters: {string.Join(", ", parameters)}");
                        Console.WriteLine($"Function body: {code}");

                        var function = new NptFunction(fnName, parameters, code);
                        variables.Add(new Dictionary<string, NptTypes.NptType>
                        {
                            { fnName, new NptTypes.NptType(NptTypes.Types.Fn, function) }
                        });

                        Console.WriteLine($"Function '{fnName}' registered successfully.");
                    }
                    else{
                        Console.WriteLine($"Processing as common variable: {variableName}");
                        //var (result, typedValue) = Help.GetType(parts[1]);
                        
                        var (resEvaluateVar, evaluatedVar) = NptStatements.EvaluateExpression(parts[1]);
                        Console.WriteLine($"{evaluatedVar.ToString()}");
                        if (resEvaluateVar != Diagnostics.Success)
                            return (null, null, resEvaluateVar);
                            
                        var (result, typedValue) = Help.GetType(evaluatedVar.ToString());
                        if (result != Diagnostics.Success && result != Diagnostics.Anomaly)
                        {
                            Console.WriteLine($"Error interpreting variable '{variableName}': {result}");
                            return (null, null, result);
                        }

                        variables.Add(new Dictionary<string, NptTypes.NptType> { { variableName, typedValue } });
                        Console.WriteLine($"Variable '{variableName}' registered with value: {typedValue}");
                    }
                }
                else{
                    Console.WriteLine($"Sintax error in ~set: '{currentLine}'");
                    return (null, null, Diagnostics.SyntaxException);
                }
            }
        }

        Console.WriteLine($">> Includes: {string.Join(", ", includes.Keys)}");
        Console.WriteLine($">> Variables: {string.Join(", ", variables.Select(v => $"{v.Keys.First()}: {v.Values.First()}"))}");

        return (includes, variables, Diagnostics.Success);
    }
}
