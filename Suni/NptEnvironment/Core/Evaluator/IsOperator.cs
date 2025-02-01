namespace Suni.Suni.NptEnvironment.Core.Evaluator;
partial class NptEvaluator
{
    internal static bool IsOperator(string token) =>
        new HashSet<string> { "&&", "||", "!", "==", "~=", ">", "<", ">=", "<=", "#", "+", "-", "*", "/", "?", "::" }
            .Contains(token);
}