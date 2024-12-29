using System.Collections.Generic;

namespace Sun.Commands;

public partial class Romance
{
    [Command("marry")]
    [InteractionInstallType(DiscordApplicationIntegrationType.GuildInstall, DiscordApplicationIntegrationType.UserInstall)]
    [InteractionAllowedContexts(DiscordInteractionContextType.Guild, DiscordInteractionContextType.PrivateChannel)]
    public static async Task Marry(CommandContext ctx,
        [Parameter("user")] DiscordUser user)
    {
        byte error = 0;
        //isnt bot:
        if (user.IsBot)
            error = 1;

        //isnt same user:
        if (user.Id == ctx.User.Id)
            error = 2;

        //users are already married
        if (new Functions.DB.DBMethods().AreUsersMarried(ctx.User.Id, user.Id)){
            error = 3;
        }

        var language = new Functions.DB.DBMethods().GetUserLanguage(ctx.User.Id, lang: null, userName:ctx.User.Username, avatar:ctx.User.AvatarUrl);
        var tr = new Globalization.Using(language);
        (string message_error, string embedTItle, string embedDescription, string content, string _, string _) = tr.Commands.GetMarryMessages(error, "", "");

        //some error:
        if (message_error != null)
        {
            await ctx.RespondAsync(new DiscordMessageBuilder()
                .WithAllowedMentions(new List<IMention> { UserMention.All })
                .WithContent($"{ctx.User.Mention} {message_error} :x:"));
            return;
        }

        //creating message
        DiscordEmbed embed = new DiscordEmbedBuilder()
            .WithTitle(embedTItle)
            .WithDescription(embedDescription);

        await ctx.RespondAsync(new DiscordMessageBuilder()
            .WithAllowedMentions(new List<IMention> { UserMention.All })
            .WithContent(content)
            .AddEmbed(embed)
            );
        
        var msg = await ctx.GetResponseAsync();
        
        await msg.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":heart:"));
    }
}
