namespace Suni.Suni.NikoSharp.Core.Evaluator;

partial class NptEvaluator
{
    internal static bool ValidateExpression(string expression)
    {
        int bc = 0;
        int bk = 0;

        foreach (char c in expression){
            if (c == '[') bc++;
            else if (c == ']') bc--;
            else if (c == '{') bk++;
            else if (c == '}') bk--;

            if (bc < 0 || bk < 0)
                return false;
        }
        return bc == 0 && bk ==0;
    }
}
