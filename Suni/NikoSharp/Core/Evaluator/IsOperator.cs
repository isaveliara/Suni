namespace Suni.Suni.NikoSharp.Core.Evaluator;
partial class NptEvaluator
{
    internal static bool IsOperator(string token) =>
        new HashSet<string> { "&&", "||", "!", "==", "~=", ">", "<", ">=", "<=", "#", "+", "-", "*", "/", "?", "::", "," }
            .Contains(token);
}