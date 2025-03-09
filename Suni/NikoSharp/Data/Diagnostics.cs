namespace Suni.Suni.NikoSharp.Data;

//enum for status
public enum Diagnostics
{
    //simple
    Success, EarlyTermination, RaisedException,

    //logic errors
    DivisionByZeroException,

    //class/object errors
    NotFoundIncludedObjectException, IncludeNotFoundException, InvalidArgsException,

    //syntax errors
    UnrecognizedLineException, InvalidKeywordException,
    OutOfRangeException,
    
    //evaluate errors
    MalformedExpression, IncompleteBinaryIFOperation, InvalidOperator,

    //language errors (errors because of me)
    UnknowException,

    //other
    InvalidTypeException,
    CannotConvertType,
    TypeMismatchException,
    BadToken,
    SyntaxException,
    FunctionNotFound,
    ArgumentMismatch,
    Forgotten, //hide the output/debug result
    UnlistedProperty,
    UnlistedVariable,
    Anomaly,
    MissingOperands,
    InvalidItemException,
}
