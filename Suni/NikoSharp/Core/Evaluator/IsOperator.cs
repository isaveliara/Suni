namespace Suni.Suni.NikoSharp.Core.Evaluator;
partial class NikoSharpEvaluator
{
    internal static bool IsOperator(string token) =>
        new HashSet<string> { "&&", "||", "!", "==", "~=", ">", "<", ">=", "<=", "#", "+", "-", "*", "/", "?", "::", "," }
            .Contains(token);
}