using Suni.Suni.NptEnvironment.Data.Types;
namespace Suni.Suni.NptEnvironment.Data;

public class CodeBlock
{
    public int IndentLevel { get; set; }
    public bool CanExecute { get; set; }
    public Dictionary<string, SType> LocalVariables { get; set; } = new();
}