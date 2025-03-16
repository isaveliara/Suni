using System.Collections.Generic;
using Suni.Suni.NikoSharp.Core;
using Suni.Suni.NikoSharp.Data.Types;

namespace Suni.Suni.NikoSharp.Data;

public class EnvironmentDataContext
{
    public string[] Tokens { get; set; }
    public Stack<CodeBlock> BlockStack { get; set; } = new();
    public List<Dictionary<string, SType>> Variables { get; set; }

    public List<string> Debugs { get; set; }
    public List<string> Outputs { get; set; }

    public EnvironmentDataContext(string[] scriptTokens, List<Dictionary<string, SType>> variables)
    {
        Tokens = scriptTokens;
        Variables = variables is not null? variables : new List<Dictionary<string, SType>> {
            new() { { "__version__", new NikosStr(SunClassBot.SuniV) } },
            new() { { "__time__", new NikosStr(DateTime.Now.ToString()) } },
        };

        Debugs = new List<string>();
        Outputs = new List<string>();

        BlockStack.Push(new CodeBlock { CanExecute = true });
        //if NikosConfigurations is not defined, set them
        if (NikoSharpConfigs.Configurations == null)
        {
            NikoSharpConfigs.SetupEnvironment();
        }
    }

    public bool TryGetVariableValue(string identifier, out SType value)
    {
        foreach (var block in BlockStack.Reverse())
            if (block.LocalVariables.TryGetValue(identifier, out value))
                return true;

        value = null;
        return false;
    }
}