namespace Sun.NPT.ScriptInterpreter
{
    //enum for status
    public enum Diagnostics
    {
        //simple
        Success, EarlyTermination, RaisedException,

        //pre-check exceptions
        DefinitionsBlockHasAnError,

        //logic errors
        DivisionByZeroException,

        //class/object errors
        NotFoundObjectException, NotFoundClassException, InvalidArgsException,

        //syntax errors
        UnrecognizedLineException, InvalidSyntaxException, InvalidKeywordException,
        OutOfRangeException,
        
        MalformedIFExpression, MissingOperandsForIFOperator, IncompleteBinaryIFOperation,

        //language errors (errors because of me)
        UnknowException,

        //npt errors
        NPTInvalidChannelException, NPTMissingPermissionsException, NPTDeniedException,
        NPTInvalidMessageException, NPTInvalidUserException,
    }
}