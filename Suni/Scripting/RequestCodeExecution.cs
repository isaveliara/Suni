using MoonSharp.Interpreter;
using Suni.Suni.NptEnvironment.Core;
using Suni.Suni.NptEnvironment.Data;
using Suni.Suni.Scripting.Lua;
namespace Suni.Suni.Scripting;

public static class Scripting
{
    public enum Languages { Lua, Npt, }

    public static async Task<(List<string> debugs, List<string> outputs, Diagnostics result)> RequestCodeExecution(ulong callerId, string code, CommandContext ctx, Languages language)
    {
        //if (callerId != 12345678910)
        //    return (null, null, Diagnostics.DeniedException);
        
        if (language == Languages.Npt)
        {
            NptSystem parser = new NptSystem(code, ctx);
            var result = await parser.ParseScriptAsync();
            return result;
        }
        else
        {
            var outputs = new List<string>();
            var debugs = new List<string>();

            using var luaManager = new LuaManager(outputs, debugs);
            Diagnostics result = luaManager.Execute(code, TimeSpan.FromSeconds(5));
            
            if (result != Diagnostics.Success)
                debugs.Add("[Script executed successfully]");
            else
                debugs.Add("[Exception Found]");

            return (debugs, outputs, result);
        }
    }
}