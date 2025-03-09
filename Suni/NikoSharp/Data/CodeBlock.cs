using Suni.Suni.NikoSharp.Data.Types;
namespace Suni.Suni.NikoSharp.Data;

public class CodeBlock
{
    public int IndentLevel { get; set; }
    public bool CanExecute { get; set; }
    public Dictionary<string, SType> LocalVariables { get; set; } = new();
}