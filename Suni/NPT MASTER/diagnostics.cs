namespace ScriptInterpreter
{
    //enum for status
    public enum Diagnostics
    {
        //simple
        Success, EarlyTermination, RaisedException, NotShowResults,

        //pre-check exceptions
        MissingONLYCASERequirement, CannotSetConstantException,

        //class/object errors
        NotFoundObjectException, NotFoundClassException, InvalidArgsException,

        //syntax errors
        UnrecognizedLineException, InvalidSyntaxException, InvalidKeywordDetectedException,

        //language errors
        UnfinishedFeatureException, UnknowException,

        //npt errors
        NPTInvalidChannelException, NPTMissingPermissionsException, NPTDeniedException,
        NPTInvalidMessageException, NPTInvalidUserException,
    }
}