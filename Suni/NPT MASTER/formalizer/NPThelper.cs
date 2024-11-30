namespace Sun.NPT.ScriptInterpreter
{
    public partial class Help
    {
        //helper method for keywords lookahead
        public static string keywordLookahead(string code, int startIndex)
        {
            int endIndex = startIndex + 1;
            while (endIndex < code.Length && char.IsLetter(code[endIndex]))
                endIndex++;
            return code.Substring(startIndex + 1, endIndex - startIndex - 1);
        }
    }
}