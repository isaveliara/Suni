
//this class is directly in the ScriptParser, to interact with it.

//usages:
//std::outputset() -> Hello World!
//std::outputadd() -> Adding a Hello World!
//std::outputclean() -> null

using System.Threading.Tasks;
using DSharpPlus.Commands;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace Sun.NPT.ScriptInterpreter
{
    //class for parser and execution
    public partial class NptSystem
    {
        //objects that interact with the class itself
        public Diagnostics STDControler(string method, List<string> args, string pointer)
        {
            switch (method)
            {
                case "nout": //std::nout() -> hello world
                    _outputs.Add(pointer);
                    break;
                case "noutset": //std::noutset() -> hello world
                    _outputs = new List<string>{pointer};
                    break;
                case "ncls"://std::ncls() -> null
                    _outputs = new List<string>();
                    break;
                case "list_var"://std::list_var() -> nil
                    _outputs.Add($">> Variables: {string.Join(", ", Variables.Select(v => $"{v.Keys.First()}: {v.Values.First()}"))}");
                    break;
                case "list_libs"://std::list_libs() -> nil
                    _outputs.Add($">> Includes: {string.Join("\n   ", Includes.Keys)}");
                    break;
                default:
                    return Diagnostics.NotFoundObjectException;
            }
            return Diagnostics.Success;
        }
    }
}