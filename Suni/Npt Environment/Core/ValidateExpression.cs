namespace Sun.NPT.ScriptInterpreter
{
    partial class NptStatements
    {
        internal static bool ValidateExpression(string expression)
        {
            int balancingBrackets = 0;

            foreach (char c in expression)
            {
                if (c == '[') balancingBrackets++;
                if (c == ']') balancingBrackets--;

                if (balancingBrackets < 0)
                    return false;
            }

            return balancingBrackets == 0;
        }
    }
}