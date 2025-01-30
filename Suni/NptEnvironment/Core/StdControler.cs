
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
using Suni.Suni.NptEnvironment.Data;

namespace Suni.Suni.NptEnvironment.Core
{
    /// <summary>
    /// class for parser and execution
    /// </summary>
    public partial class NptSystem
    {
        public static readonly List<string> MainControlerLibMethods = new List<string> { "nout", "noutset", "ncls", "list_var", "list_libs" };

        /// <summary>
        /// objects that interact with the main class itself (the parser)
        /// </summary>
        public Diagnostics STDControler(string method, List<string> args, string pointer)
        {
            switch (method)
            {
                case "nout": //std::nout() -> hello world
                    ContextData.Outputs.Add(pointer);
                    break;
                case "noutset": //std::noutset() -> hello world
                    ContextData.Outputs = new List<string>{pointer};
                    break;
                case "ncls"://std::ncls() -> null
                    ContextData.Outputs = new List<string>();
                    break;
                case "list_var"://std::list_var() -> nil
                    ContextData.Outputs.Add($">> Variables: {string.Join(", ", ContextData.Variables.Select(v => $"{v.Keys.First()}: {v.Values.First()}"))}");
                    break;
                case "list_libs"://std::list_libs() -> nil
                    ContextData.Outputs.Add($">> Includes: {string.Join("\n   ", ContextData.Includes.Keys)}");
                    break;
                default:
                    return Diagnostics.NotFoundIncludedObjectException;
            }
            return Diagnostics.Success;
        }
    }
}