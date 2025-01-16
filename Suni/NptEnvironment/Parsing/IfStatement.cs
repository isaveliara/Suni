namespace Sun.NPT.ScriptInterpreter
{
    public partial class NptStatements
    {
        public static (Diagnostics, bool) IFStatement(string condition)
        {
            var (r, conditionResult) = EvaluateExpression(condition);
            //verificar se o valor de conditionResulté bool
            if (conditionResult is not bool)
                return (Diagnostics.CannotConvertType, false);

            return (r, (bool)conditionResult);
        }
    }
}