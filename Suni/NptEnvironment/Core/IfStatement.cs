using Suni.Suni.NptEnvironment.Data;
using Suni.Suni.NptEnvironment.Data.Types;
namespace Suni.Suni.NptEnvironment.Core;

public partial class NptSystem
{
    public static (Diagnostics, bool) IFStatement(string condition)
    {
        var (conditionResult, r, msg) = NptSystem.EvaluateExpression(condition);
        //verificar se o valor de conditionResulté bool
        if (conditionResult is not NptBool)
            return (Diagnostics.CannotConvertType, false);

        return (r, (bool)conditionResult.Value);
    }
}
