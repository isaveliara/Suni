namespace Suni.Suni.NptEnvironment.Syntax
{
    public abstract class StatementSyntax
    {
        public string OriginalLine { get; }
        
        protected StatementSyntax(string originalLine)
        {
            OriginalLine = originalLine;
        }
    }
}
