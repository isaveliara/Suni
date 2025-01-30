namespace Suni.Suni.NptEnvironment.Data;

public class CodeBlock
{
    public int IndentLevel { get; set; }
    public List<string> Lines { get; } = [];
    public bool CanExecute { get; set; } = true;
    public string Condition { get; set; } = null;
}