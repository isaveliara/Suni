using System.Collections.Generic;
using Sun.Dimensions.Romance;

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
        if (new Sun.Functions.DB.DBMethods().AreUsersMarried(ctx.User.Id, user.Id)){
            //message_error = $"";
            error = 3;
        }

        var language = Functions.DB.DBMethods.tryFoundUserLang(ctx.User.Id, lang: null, userName:ctx.User.Username, avatar:ctx.User.AvatarUrl);
        var tr = new Globalization.Using(language);
        (string message_error, string embedTItle, string embedDescription, string content, string noMoney, string success) = tr.Commands.GetMarryMessages(error, ctx.User.Mention, user.Mention);

        //some error:
        if (message_error != null)
        {
            await ctx.Channel.SendMessageAsync($"{ctx.User.Mention} {message_error} :x:");
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

        //event
        bool re = RomanceMethods.MarryUsers(ctx.User.Id, user.Id, true);
        if (!re)
        {
            await ctx.RespondAsync(noMoney);
            return;
        }

        await ctx.RespondAsync(new DiscordMessageBuilder()
            .WithAllowedMentions(new List<IMention> { UserMention.All })
            .WithContent(success));
    }
}
