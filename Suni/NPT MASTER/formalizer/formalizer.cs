using DSharpPlus.Commands;
using System.Collections.Generic;
using Sun.NPT.ScriptInterpreter;

namespace Sun.NPT.ScriptFormalizer
{
    public partial class JoinScript
    {
        public (List<string>, Dictionary<string, List<string>>, List<Dictionary<string, NptSystem.NptType>>, Diagnostics) JoinHere(string code, CommandContext ctx)
        {
            var (result, resultp) = SetPlaceHolders(code, ctx);
            if (resultp != Diagnostics.Success)
                return (null, null, null, resultp);
            
            var (formalized, definitionsBlock, resultf) = Formalizer(result);
            if (resultf != Diagnostics.Success)
                return (null, null, null, resultf);
            
            var (libraries, variables, resultDefsInterp) = InterpretDefinitionsBlock(definitionsBlock);
            if (resultDefsInterp != Diagnostics.Success)
                return (null, null, null, resultDefsInterp);

            return (formalized, libraries, variables, Diagnostics.Success);
        }

        private (List<string>, List<string>, Diagnostics) Formalizer(string code)
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
                    if (keyword.Chars == "definitions")
                    {
                        inDefinitionsBlock = true;
                        i += keyword.Chars.Length;  // Avançar o índice
                        currentLine = "";  // Limpar linha
                        continue;
                    }
                    else if (keyword.Chars == "ends")
                    {
                        inDefinitionsBlock = false;
                        i += keyword.Chars.Length;  // Avançar o índice
                        currentLine = "";  // Limpar linha
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
            System.Console.WriteLine($"suni instruction\n>\n    ]{string.Join("\n    ]", Deflines)}\n<");
            System.Console.WriteLine($"suni query\n>\n    ]{string.Join("\n    ]", lines)}\n<\n");

            //we can just ignore this
            //if (!hasDefinitionsBlock)
            //    return (null, Diagnostics.NotFoundDefinitionsBlock);
            
            return (lines, Deflines, Diagnostics.Success);
        }
    }
}