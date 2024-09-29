using DSharpPlus.CommandsNext;
using System.Collections.Generic;
using System.Text;
using ScriptInterpreter;

namespace ScriptFormalizer
{
    public class JoinScript
    {
        public (string, Diagnostics) JoinHere(string code, CommandContext ctx)
        {
            var (formalized, resultf) = Formalizer(code);
            if (resultf != Diagnostics.Success)
                return (null, Diagnostics.InvalidSyntaxException); //formalize includes testing syntax

            var (result, resultp) = SetPlaceHolders(formalized, ctx);
            if (resultp != Diagnostics.Success)
                return (null, Diagnostics.UnknowException); //maybe do something if a invalid placeholder exists? &{}
            
            return (result, Diagnostics.Success);
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

        private (string, Diagnostics) Formalizer(string code)
                => (code, Diagnostics.Success);
    }
}