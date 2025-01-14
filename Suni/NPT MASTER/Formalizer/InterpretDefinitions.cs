using System.Collections.Generic;
using Sun.NPT.ScriptInterpreter;
using System.Linq;
using System.Text.RegularExpressions;
using static Sun.NPT.ScriptInterpreter.NptSystem;

namespace Sun.NPT.ScriptFormalizer
{
    public partial class JoinScript
    {
        internal static (Dictionary<string, List<string>> includes, List<Dictionary<string, NptSystem.NptType>> variables, Diagnostics) InterpretDefinitionsBlock(List<string> lines)
        {
            //(default)
            var includes = new Dictionary<string, List<string>>{
                { "npt", new List<string>{"log", "ban", "unban", "react"} },
            };
            var variables = new List<Dictionary<string, NptSystem.NptType>>{
                new Dictionary<string, NptSystem.NptType> { { "__version__", new NptSystem.NptType(NptSystem.Types.Str, Bot.SunClassBot.SuniV) } },
                new Dictionary<string, NptSystem.NptType> { { "__time__", new NptSystem.NptType(NptSystem.Types.Str, System.DateTime.Now.ToString()) } }
            };

            for (int i = 0; i < lines.Count; i++)
            {
                string currentLine = lines[i].Trim();

                //process "include" statements
                if (currentLine.StartsWith("~include"))
                {
                    var includeName = currentLine.Substring(9).Trim();
                    //DEBUG:
                    System.Console.WriteLine(includeName + " included by line: " + currentLine);
                    if (!string.IsNullOrWhiteSpace(includeName) && !includes.ContainsKey(includeName))
                        switch (includeName) //TODO: get dinamically the methods of the libraries
                        {
                            case "std":
                                includes[includeName] = new List<string> { "nout", "noutset", "ncls", "list_var", "list_libs" };
                                break;
                            default:
                                includes[includeName] = new List<string>(); //add the include to the dictionary
                                break;
                        }
                }

                //process "set" statements for variables
                if (currentLine.StartsWith("~set"))
                {
                    var parts = currentLine.Substring(4).Trim().Split(' ', 2);
                    if (parts.Length == 2)
                    {
                        string variableName = parts[0].Trim();
                        Console.WriteLine($"Detectado ~set para variável ou função: {variableName}");

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
                            variables.Add(new Dictionary<string, NptType>
                            {
                                { fnName, new NptType(Types.Fn, function) }
                            });

                            Console.WriteLine($"Function '{fnName}' registered successfully.");
                        }
                        else
                        {
                            Console.WriteLine($"Processing as common variable: {variableName}");
                            var (result, typedValue) = Help.GetType(parts[1]);
                            if (result != Diagnostics.Success)
                            {
                                Console.WriteLine($"Error interpreting variable '{variableName}': {result}");
                                return (null, null, result);
                            }

                            variables.Add(new Dictionary<string, NptType> { { variableName, typedValue } });
                            Console.WriteLine($"Variable '{variableName}' registered with value: {typedValue}");
                        }
                    }
                    else
                    {
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
}