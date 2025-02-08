using Suni.Suni.NptEnvironment.Data;
namespace Suni.Suni.NptEnvironment.Processors;

public interface IStatementProcessor
{
    bool TryProcess(string line, out Diagnostics diagnostics);
}
