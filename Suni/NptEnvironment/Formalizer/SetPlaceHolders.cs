using Suni.Suni.NptEnvironment.Data;
namespace Suni.Suni.NptEnvironment.Formalizer;
public partial class FormalizingScript
{
    /// <summary>
    /// Sets the placeholders on the lines. If no placeholder is identified, it just returns the value without any changes.
    /// </summary>
    /// <param name="lines"></param>
    /// <param name="DiscordCtx"></param>
    /// <returns></returns>
    internal (Diagnostics diagnostic, string diagnosticMessage) SetPlaceHolders()
    {
        if (DiscordCtx is null)
            return (Diagnostics.Anomaly, "Unable to identify placeholders.");

        //nullable values
        var userNick = DiscordCtx.Member?.Nickname ?? "nil";
        var channelParent = DiscordCtx.Channel.Parent?.Name ?? "nil";
        var channelParentId = DiscordCtx.Channel.Parent?.Id.ToString() ?? "nil";
        var channelParentName = DiscordCtx.Channel.Parent?.Name ?? "nil";
        var guildName = DiscordCtx.Guild?.Name ?? "Direct Messages";
        var guildId = DiscordCtx.Guild?.Id.ToString() ?? "nil";
        var guildMemberCount = DiscordCtx.Guild?.MemberCount.ToString() ?? "0";

        //dictionary of placeholders
        var placeholders = new Dictionary<string, string>
        {
            // user
            { "&{userId}", DiscordCtx.User.Id.ToString() },
            { "&{userMention}", DiscordCtx.User.Mention },
            { "&{userName}", DiscordCtx.User.Username },
            { "&{userNick}", userNick },

            //channel items
            { "&{channel}", DiscordCtx.Channel.Name },
            { "&{channelName}", DiscordCtx.Channel.Name },
            { "&{channelId}", DiscordCtx.Channel.Id.ToString() },
            { "&{channelParent}", channelParent },
            { "&{channelParentId}", channelParentId },
            { "&{channelParentName}", channelParentName },

            //guild
            { "&{guild}", guildName },
            { "&{guildId}", guildId },
            { "&{guildName}", guildName },
            { "&{guildMembers}", guildMemberCount }
        };

        //replacing values in each line
        for (int i = 0; i < FormalizingDataContext.Lines.Count; i++)
            foreach (var placeholder in placeholders)
                FormalizingDataContext.Lines[i] = FormalizingDataContext.Lines[i].Replace(placeholder.Key, placeholder.Value);

        return (Diagnostics.Success, null);
    }
}
