using System.Collections.Generic;
using System.Text;
using Suni.Suni.NptEnvironment.Data;

namespace Suni.Suni.NptEnvironment.Formalizer
{
    public partial class FormalizingScript
    {
        internal static (string, Diagnostics) SetPlaceHolders(string script, CommandContext ctx)
        {
            //stringBuilder for building the script
            var sb = new StringBuilder(script);

            //nullable values
            var userNick = ctx.Member?.Nickname ?? "nil";
            var channelParent = ctx.Channel.Parent?.Name ?? "nil";
            var channelParentId = ctx.Channel.Parent?.Id.ToString() ?? "nil";
            var channelParentName = ctx.Channel.Parent?.Name ?? "nil";

            //nullable guild values
            var guildName = ctx.Guild?.Name ?? "Direct Messages";
            var guildId = ctx.Guild?.Id.ToString() ?? "nil";
            var guildMemberCount = ctx.Guild?.MemberCount.ToString() ?? "0";

            //dict of placeholders
            var placeholders = new Dictionary<string, string>
            {
                //user
                { "&{userId}", ctx.User.Id.ToString() },
                { "&{userMention}", ctx.User.Mention },
                { "&{userName}", ctx.User.Username },
                { "&{userNick}", userNick },

                //channel items
                { "&{channel}", ctx.Channel.Name },
                { "&{channelName}", ctx.Channel.Name },
                { "&{channelId}", ctx.Channel.Id.ToString() },
                { "&{channelParent}", channelParent },
                { "&{channelParentId}", channelParentId },
                { "&{channelParentName}", channelParentName },

                //guild
                { "&{guild}", guildName },
                { "&{guildId}", guildId },
                { "&{guildName}", guildName },
                { "&{guildMembers}", guildMemberCount }
            };

            //replacing values
            foreach (var placeholder in placeholders)
                sb.Replace(placeholder.Key, placeholder.Value);

            return (sb.ToString(), Diagnostics.Success);
        }
    }
}