using System.Text.RegularExpressions;
using Suni.Suni.NikoSharp.Data;
using Suni.Suni.NikoSharp.Data.Types;

namespace Suni.Suni.NikoSharp.Core;

public partial class Help
{
    //helper method for keywords lookahead
    public static (string Letters, string Chars) keywordLookahead(string code, int startIndex = 0)
    {
        int currentIndex = startIndex + 1;
        int firstNonLetterIndex = currentIndex;

        while (currentIndex < code.Length && code[currentIndex] != ' ' && code[currentIndex] != '\n')
        {
            if (char.IsLetter(code[currentIndex]) && firstNonLetterIndex == currentIndex)
                firstNonLetterIndex++;
            currentIndex++;
        }

        string letters = code.Substring(startIndex + 1, firstNonLetterIndex - startIndex - 1);
        string chars = code.Substring(startIndex + 1, currentIndex - startIndex - 1);

        return (letters, chars);
    }
}
