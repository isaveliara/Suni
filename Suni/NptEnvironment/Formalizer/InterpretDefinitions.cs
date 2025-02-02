using System.Collections.Generic;
using Suni.Suni.NptEnvironment.Data;
using System.Reflection;
using Suni.Suni.NptEnvironment.Core;
using Suni.Suni.NptEnvironment.Data.Types;
using Suni.Suni.NptEnvironment.Syntax;
using Suni.Suni.NptEnvironment.Core.Evaluator;
namespace Suni.Suni.NptEnvironment.Formalizer;

public partial class FormalizingScript
{
    internal void InterpretDefinitionsBlock()
    {
        //(default)
        FormalizingDataContext.Includes = new Dictionary<string, List<string>>{
            { "std", NptSystem.MainControlerLibMethods }
        };
        var variables = new List<Dictionary<string, SType>>{
            new Dictionary<string, SType> { { "__version__", new NptStr(SunClassBot.SuniV) } },
            new Dictionary<string, SType> { { "__time__", new NptStr(DateTime.Now.ToString()) } }
        };

        for (int i = 0; i < DefLines.Count; i++)
        {
            string currentLine = DefLines[i].Trim();
            if (currentLine.StartsWith('~'))
            {
                var keyWord = Help.keywordLookahead(currentLine).Letters;
                Console.WriteLine($"Identified DefLine keyword: >>{keyWord}<<");
                
                switch (keyWord)
                {
                    case "include":
                        var includeName = currentLine.Substring(9).Trim();
                        Console.WriteLine($"{includeName} included by line: {currentLine}");

                        if (!string.IsNullOrWhiteSpace(includeName) && !FormalizingDataContext.Includes.ContainsKey(includeName)){
                            string classNameProper = char.ToUpper(includeName[0]) + includeName.Substring(1).ToLower() + "Entitie";
                            Type type = Type.GetType($"Sun.NPT.ScriptInterpreter.{classNameProper}");

                            if (type != null){
                                var libMethodsProperty = type.GetProperty("LibMethods", BindingFlags.Static | BindingFlags.Public);
                                if (libMethodsProperty != null){
                                    if (libMethodsProperty.GetValue(null) is List<string> methods && methods.Any())
                                    {
                                        FormalizingDataContext.Includes[includeName] = methods;
                                        Console.WriteLine($"Library '{includeName}' registered with methods: {string.Join(", ", methods)}");
                                    }
                                    else
                                    {
                                        FormalizingDataContext.Includes[includeName] = new List<string>();
                                        Console.WriteLine($"Library '{includeName}' registered but no methods found.");
                                    }
                                }
                                else{
                                    FormalizingDataContext.Includes[includeName] = new List<string>();
                                    Console.WriteLine($"Library '{includeName}' found, but 'LibMethods' property is missing.");
                                }
                            }
                            else{
                                Console.WriteLine($"Error: Library '{includeName}' class '{classNameProper}' not found.");
                                FormalizingDataContext.LogDiagnostic(Diagnostics.IncludeNotFoundException, $"Library '{includeName}' class '{classNameProper}' not found.");
                                return;
                            }
                        }
                        break;
                    default:
                    if (Enum.TryParse(keyWord, true, out STypes varType))
                    {
                        //Float set pi 3,14.
                        //if a type is placed as 'instruction', it means it is a variable declaration
                        string rest = currentLine.Substring(keyWord.Length + 1).Trim();
                        if (rest.StartsWith("set"))
                        {
                            rest = rest[4..];
                            var parts = rest.Split(' ');
                            if (parts.Length < 2)
                                FormalizingDataContext.LogDiagnostic(Diagnostics.SyntaxException, $"Missing value for variable '{rest}'");

                            string varName = parts[0];
                            string stringVarValue = parts[1];
                            var evaluationResults = NptEvaluator.EvaluateExpression(stringVarValue, FormalizingDataContext);
                            if (evaluationResults.diagnostic != Diagnostics.Success)
                                FormalizingDataContext.LogDiagnostic(evaluationResults.diagnostic, evaluationResults.diagnosticMessage);

                            //setting
                            VariableDeclarationSyntax.TryParse(varName, varType, evaluationResults.resultValue, FormalizingDataContext);
                        }
                        else
                        FormalizingDataContext.LogDiagnostic(Diagnostics.SyntaxException, $"After a '{varType}' statement, is expected a 'set' expression.");
                    }
                    else
                        FormalizingDataContext.LogDiagnostic(Diagnostics.SyntaxException, $"'{keyWord}' isn't a valid instruction to DefinitionsLines.");
                    break;
                }
            }
        }

        Console.WriteLine($">> Includes: {string.Join(", ", FormalizingDataContext.Includes.Keys)}");
        Console.WriteLine($">> Variables: {string.Join(", ", FormalizingDataContext.Variables.Select(v => $"{v.Keys.First()}: {v.Values.First()}"))}");
        
        return;//return (includes, variables, Diagnostics.Success, null);
    }
}
