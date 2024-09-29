//usages:
//master::outputset() -> Hello World!
//master::outputadd() -> Adding a Hello World!
//master::outputclean() -> null

using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace ScriptInterpreter
{
    //class for parser and execution
    public partial class ScriptParser
    {
        //objects that interact with the class itself
        public Diagnostics MASTERControler(string method, List<string> args, string pointer)
        {
            switch (method)
            {
                case "outputadd": //master::outputadd() -> hello world
                    _outputs.Add(pointer);
                    break;
                case "outputset": //master::outputset() -> hello world
                    _outputs = new List<string>{pointer};
                    break;
                case "outputclean"://master::outputadd() -> null
                    _outputs = new List<string>();
                    break;
                default:
                    return Diagnostics.NotFoundObjectException;
            }
            return Diagnostics.Success;
        }
    }
}