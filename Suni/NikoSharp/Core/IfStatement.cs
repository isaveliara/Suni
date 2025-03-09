using Suni.Suni.NikoSharp.Core.Evaluator;
using Suni.Suni.NikoSharp.Data;
using Suni.Suni.NikoSharp.Data.Types;
namespace Suni.Suni.NikoSharp.Core;

public partial class NptSystem
{
    public static (Diagnostics, bool) IFStatement(string condition)
    {
        var (conditionResult, r, msg) = NptEvaluator.EvaluateExpression(condition);
        //verificar se o valor de conditionResulté bool
        if (conditionResult is not NptBool)
            return (Diagnostics.CannotConvertType, false);

        return (r, (bool)conditionResult.Value);
    }
}
