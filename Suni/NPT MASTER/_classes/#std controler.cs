
//this class is directly in the ScriptParser, to interact with it.

//usages:
//std::outputset() -> Hello World!
//std::outputadd() -> Adding a Hello World!
//std::outputclean() -> null

using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace Sun.NPT.ScriptInterpreter
{
    //class for parser and execution
    public partial class ScriptParser
    {
        //objects that interact with the class itself
        public Diagnostics STDControler(string method, List<string> args, string pointer)
        {
            switch (method)
            {
                case "outputadd": //std::outputadd() -> hello world
                    _outputs.Add(pointer);
                    break;
                case "outputset": //std::outputset() -> hello world
                    _outputs = new List<string>{pointer};
                    break;
                case "outputclean"://std::outputadd() -> null
                    _outputs = new List<string>();
                    break;
                case "script_variables":
                    _outputs.Add($">> Variables: {string.Join(", ", _variables.Select(v => $"{v.Keys.First()}: {v.Values.First()}"))}");
                    break;
                case "script_includes":
                    _outputs.Add($">> Includes: {string.Join("\n   ", _includes.Keys)}");
                    break;
                default:
                    return Diagnostics.NotFoundObjectException;
            }
            return Diagnostics.Success;
        }
    }
}