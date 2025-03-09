namespace Suni.Suni.NikoSharp.Formalizer;

public partial class FormalizingScript
{
    /// <summary>
    /// Splits the string code int List<string> lines, List<string> defLines; discarding comments.
    /// </summary>
    /// <param name="code"></param>
    /// <param name="separators"></param>
    /// <returns></returns>
    internal static List<string> SplitCode(string code)
    {
        //split the lines of script into list
        bool isString = false;

        List<string> lines = new List<string>();
        string currentLine = "";

        for (int i = 0; i < code.Length; i++)
        {
            //toggle string mode when encountering a quote
            char currentChar = code[i];
            if (currentChar == '"'){
                isString = !isString;//toggle
                currentLine += currentChar;
                continue;
            }

            //comments (--) outside of strings
            if (!isString && currentChar == '-' && i + 1 < code.Length && code[i + 1] == '-')
            {
                //skip rest of line until "\n"
                while (i < code.Length && code[i] != '\n')
                        i++;
                
                continue;
            }

            //split on newline "\n" or on '.' outside of strings
            if (!isString && (currentChar == '\n' || currentChar == '.')){
                if (!string.IsNullOrWhiteSpace(currentLine)) //add line for definitions or normal code
                    lines.Add(currentLine.Trim());
                
                currentLine = "";
                continue;
            }

            //add character to current segment
            currentLine += currentChar;
        }

        //add any remaining text as the last line
        if (!string.IsNullOrWhiteSpace(currentLine))
            lines.Add(currentLine.Trim());
        
        //display (DEBUGGING)
        Console.WriteLine($"suni query\n>\n    ]{string.Join("\n    ]", lines)}\n<\n");
        
        return lines;
    }
}