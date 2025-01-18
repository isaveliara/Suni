using System.Collections.Generic;
namespace Suni.Suni.Commands;

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
        if (new DBMethods().AreUsersMarried(ctx.User.Id, user.Id)){
            error = 3;
        }

        var solve = await SolveLang.SolveLangAsync(ctx:ctx);
        var (message_error, embedTitle, embedDescription, content, _, _) = solve.Commands.GetMarryMessages(error, "de", "j");

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
            .WithTitle(embedTitle)
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
