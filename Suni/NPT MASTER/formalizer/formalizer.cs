using DSharpPlus.CommandsNext;
using System.Collections.Generic;
using System.Text;
using ScriptInterpreter;

namespace ScriptFormalizer
{
    public class JoinScript
    {
        public (List<string>, Diagnostics) JoinHere(string code, CommandContext ctx)
        {
            var (result, resultp) = SetPlaceHolders(code, ctx);
            if (resultp != Diagnostics.Success)
                return (null, Diagnostics.UnknowException); //maybe do something if a invalid placeholder exists? &{}
            
            var (formalized, resultf) = Formalizer(result);
            if (resultf != Diagnostics.Success)
                return (null, Diagnostics.InvalidSyntaxException);

            return (formalized, Diagnostics.Success);
        }

        private (string, Diagnostics) SetPlaceHolders(string script, CommandContext ctx)
        {
            //string builder
            var sb = new StringBuilder(script);

            //possible null values
            var userNick = ctx.Guild.GetMemberAsync(ctx.Message.Author.Id).Result?.Nickname ?? ctx.Message.Author.Username;
            var channelFather = ctx.Channel.Parent?.Name ?? "null";
            var channelFatherId = ctx.Channel.Parent?.Id.ToString() ?? "null"; 
            var channelFatherName = ctx.Channel.Parent?.Name ?? "null";

            //dict
            var placeholders = new Dictionary<string, string>
            {
                //user
                { "&{user}", ctx.Message.Author.Username },
                { "&{userId}", ctx.Message.Author.Id.ToString() },
                { "&{userMention}", ctx.Message.Author.Mention },
                { "&{userName}", ctx.Message.Author.Username },
                { "&{userNick}", userNick },

                //server items
                { "&{channel}", ctx.Message.Channel.Name },
                { "&{channelName}", ctx.Message.Channel.Name },
                { "&{channelId}", ctx.Message.Channel.Id.ToString() },
                { "&{channelFather}", channelFather },
                { "&{channelFatherId}", channelFatherId },
                { "&{channelFatherName}", channelFatherName },

                //guild
                { "&{guild}", ctx.Guild.Name },
                { "&{guildId}", ctx.Guild.Id.ToString() },
                { "&{guildName}", ctx.Guild.Name },
                { "&{guildMembers}", ctx.Guild.MemberCount.ToString() }
            };

            //replacing
            foreach (var placeholder in placeholders)
                sb.Replace(placeholder.Key, placeholder.Value);

            return (sb.ToString(), Diagnostics.Success);
        }

        private (List<string>, Diagnostics) Formalizer(string code)
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

                //split on "." outside of strings
                if (!isString && currentChar == '.'){
                    if (!string.IsNullOrWhiteSpace(currentLine)) //add line
                            lines.Add(currentLine.Trim());
                    
                    currentLine = "";
                    continue;
                }

                //split on newline "\n" outside of strings
                if (!isString && currentChar == '\n'){
                    if (!string.IsNullOrWhiteSpace(currentLine)) //add line
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
            System.Console.WriteLine($">\n    ]{string.Join("\n    ]", lines)}\n<");
            
            return (lines, Diagnostics.Success);
        }
    }
}