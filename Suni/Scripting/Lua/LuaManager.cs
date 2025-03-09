using MoonSharp.Interpreter;
using Suni.Suni.NikoSharp.Data;
namespace Suni.Suni.Scripting.Lua;

public sealed class LuaManager : IDisposable
{
    private readonly Script _script;
    private CommandContext _ctx;
    private readonly List<string> _outputs;

    public LuaManager(List<string> outputs, CommandContext ctx)
    {
        _outputs = outputs ?? new List<string>();
        _script = new Script();
        _ctx = ctx;

        ConfigureScript();
    }

    private void ConfigureScript()
    {
        // Remove funções perigosas
        string[] denyFuncs = { "os", "io", "debug", "load", "require", "dofile", "collectgarbage" };
        foreach (string func in denyFuncs)
            _script.Globals[func] = null;

        _script.Globals["print"] = (Action<string>)(msg => _outputs.Add(msg));
        
        _script.Globals["SuniApi"] = UserData.Create(new SuniApi.SuniApi(_ctx));
    }

    public Diagnostics Execute(string luaCode, TimeSpan timeout)
    {
        try
        {
            var task = Task.Run(() => _script.DoString(luaCode));
            
            if (!task.Wait(timeout))
            {
                _outputs.Add("Timeout: Script excedeu o tempo limite.");
                return Diagnostics.UnknowException;
            }

            DynValue result = task.Result;
            
            if (result.IsNotNil())
            {
                _outputs.Add(result.ToPrintString());
                return Diagnostics.Success;
            }

            return Diagnostics.Success; // Mesmo sem retorno, execução bem-sucedida
        }
        catch (ScriptRuntimeException ex)
        {
            _outputs.Add($"Erro de execução: {ex.Message}");
            return Diagnostics.UnknowException;
        }
        catch (SyntaxErrorException ex)
        {
            _outputs.Add($"Erro de sintaxe: {ex.Message}");
            return Diagnostics.SyntaxException;
        }
        catch (Exception ex)
        {
            _outputs.Add($"Erro inesperado: {ex.Message}");
            return Diagnostics.UnknowException;
        }
    }

    public void Dispose() => _script?.Globals?.Clear();
}
