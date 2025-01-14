namespace Sun.NPT.ScriptInterpreter;

//enum for status
public enum Diagnostics
{
    //simple
    Success, EarlyTermination, RaisedException,

    //logic errors
    DivisionByZeroException,

    //class/object errors
    NotFoundObjectException, NotFoundClassException, InvalidArgsException,

    //syntax errors
    UnrecognizedLineException, InvalidKeywordException,
    OutOfRangeException,
    
    //evaluate errors
    MalformedExpression, MissingOperandsForIFOperator, IncompleteBinaryIFOperation, InvalidOperator,

    //language errors (errors because of me)
    UnknowException,

    //other/npt errors
    NPTInvalidChannelException, NPTMissingPermissionsException, NPTDeniedException,
    NPTInvalidMessageException, NPTInvalidUserException,
    UnknowTypeException,
    CannotConvertType,
    TypeMismatchException,
    MissingOperandsForEvaluation,
    BadToken,
    SyntaxException,
    FunctionNotFound,
    ArgumentMismatch,
}
