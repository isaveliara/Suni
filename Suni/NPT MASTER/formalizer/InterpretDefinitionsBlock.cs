using System.Collections.Generic;
using Sun.NPT.ScriptInterpreter;
using System.Linq;

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
                new Dictionary<string, NptSystem.NptType> { { "__version__", new NptSystem.NptType(NptSystem.Types.Str, "suninstruction_1.0.1a") } },
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
                        var typedValue = Help.GetType(parts[1]);

                        //add the variable with its typed value
                        variables.Add(new Dictionary<string, NptSystem.NptType> { { variableName, typedValue } });
                        System.Console.WriteLine($"Variable '{variableName}' set with type '{typedValue.Type}' and value '{typedValue.Value}'");
                    }
                }
            }


            System.Console.WriteLine($">> Includes: {string.Join(", ", includes.Keys)}");
            System.Console.WriteLine($">> Variables: {string.Join(", ", variables.Select(v => $"{v.Keys.First()}: {v.Values.First()}"))}");

            return (includes, variables, Diagnostics.Success);
        }
    }
}