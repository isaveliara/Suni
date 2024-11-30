using System.Collections.Generic;
using Sun.NPT.ScriptInterpreter;
using System.Linq;

namespace Sun.NPT.ScriptFormalizer
{
    public partial class JoinScript
    {
        internal static (Dictionary<string, List<string>> includes, List<Dictionary<string, object>> variables, Diagnostics) InterpretDefinitionsBlock(List<string> lines)
        {
            //(default)
            var includes = new Dictionary<string, List<string>>{
                { "npt", new List<string>{"log", "ban", "unban", "react"} },
            };
            var variables = new List<Dictionary<string, object>>{
                new Dictionary<string, object> { {"__version__", "suninstruction_1.0.0"} },
                new Dictionary<string, object> { {"__time__", System.DateTime.Now}}
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
                                includes[includeName] = new List<string> { "outputadd", "outputset", "outputclean", "script_variables", "script_includes" };
                                break;
                            default:
                                includes[includeName] = new List<string>(); //add the include to the dictionary
                                break;
                        }
                }

                //process "set" statements for variables
                if (currentLine.StartsWith("~set"))
                {
                    //example: set __version__ "1.0.0ret"
                    var parts = currentLine.Substring("~set".Length).Trim().Split([' '], 2);
                    if (parts.Length == 2)
                    {
                        string variableName = parts[0].Trim();
                        string value = parts[1].Trim(' ', '"'); //remove any surrounding quotes

                        //save the variable into the dictionary
                        variables.Add(new Dictionary<string, object> { { variableName, value } });
                        System.Console.WriteLine($"type of var '{variableName}' is {value.GetType()} - (value '{value}')");
                    }
                }
            }


            System.Console.WriteLine($">> Includes: {string.Join(", ", includes.Keys)}");
            System.Console.WriteLine($">> Variables: {string.Join(", ", variables.Select(v => $"{v.Keys.First()}: {v.Values.First()}"))}");

            return (includes, variables, Diagnostics.Success);
        }
    }
}