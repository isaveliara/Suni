using System.Collections.Generic;
using Suni.Suni.NptEnvironment.Core;
using Suni.Suni.NptEnvironment.Data;
using Suni.Suni.NptEnvironment.Data.Types;

namespace Suni.Suni.NptEnvironment.Formalizer;

public partial class FormalizingScript
{
    /// <summary>
    /// Splits the string code int List<string> lines, List<string> defLines; discarding comments.
    /// </summary>
    /// <param name="code"></param>
    /// <param name="separators"></param>
    /// <returns></returns>
    internal static (List<string> lines, List<string> defLines) SplitCode(string code)
    {
        //split the lines of script into list

        bool isString = false;
        bool inDefinitionsBlock = false; 
        //bool hasDefinitionsBlock = false;

        List<string> lines = new List<string>();
        List<string> Deflines = new List<string>();

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
            if (!isString && currentChar == '#')
            {
                var keyword = Help.keywordLookahead(code, i);
                if (keyword.Letters == "definitions")
                {
                    inDefinitionsBlock = true;
                    i += keyword.Letters.Length;  //advance index
                    currentLine = "";  //clean line
                    continue;
                }
                else if (keyword.Letters == "ends")
                {
                    inDefinitionsBlock = false;
                    i += keyword.Letters.Length;  //advance index
                    currentLine = "";  //clean line
                    continue;
                }
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
                    if (inDefinitionsBlock)
                        Deflines.Add(currentLine);
                    else
                        lines.Add(currentLine.Trim());
                
                currentLine = "";
                continue;
            }

            //add character to current segment
            currentLine += currentChar;
        }

        //add any remaining text as the last line
        if (!string.IsNullOrWhiteSpace(currentLine))
            if (inDefinitionsBlock)
                Deflines.Add(currentLine);
            else
                lines.Add(currentLine.Trim());
        
        //display (DEBUGGING)
        Console.WriteLine($"suni instruction\n>\n    ]{string.Join("\n    ]", Deflines)}\n<");
        Console.WriteLine($"suni query\n>\n    ]{string.Join("\n    ]", lines)}\n<\n");
        
        return (lines, Deflines);
    }
}