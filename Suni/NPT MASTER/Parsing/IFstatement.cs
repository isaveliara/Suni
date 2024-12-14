namespace Sun.NPT.ScriptInterpreter
{
    public partial class NptStatements
    {
        public static (Diagnostics, bool) IFStatement(string condition)
        {
            var (r, conditionResult) = EvaluateExpression(condition);
            return (r, conditionResult);
        }
    }
}