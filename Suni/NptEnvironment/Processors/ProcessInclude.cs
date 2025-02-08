using Suni.Suni.NptEnvironment.Data;
namespace Suni.Suni.NptEnvironment.Processors;

public class IncludeProcessor : IStatementProcessor
{
    public bool TryProcess(string line, out Diagnostics diagnostics)
    {
        diagnostics = Diagnostics.Success;
        return true;
    }
}
