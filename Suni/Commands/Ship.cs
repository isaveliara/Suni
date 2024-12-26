using System.Collections.Generic;
using Sun.Dimensions.Romance;

namespace Sun.Commands;

public partial class Romance
{
    [Command("ship")]
    [InteractionInstallType(DiscordApplicationIntegrationType.GuildInstall, DiscordApplicationIntegrationType.UserInstall)]
    [InteractionAllowedContexts(DiscordInteractionContextType.Guild, DiscordInteractionContextType.PrivateChannel)]
    public static async Task Ship(CommandContext ctx,
        [Parameter("user1")] DiscordUser user2,
        [Parameter("user2")] DiscordUser user1 = null)
    {
        user2 ??= ctx.User; if (user1 == null) user1 = ctx.User;//defaults
        int percent = (user1.Username + user2.Username).Where(char.IsLetter).Sum(letra => char.ToLower(letra));
        percent = (percent+50) % 100 + 1;
        
        //translation of ship messages
        var language = Functions.DB.DBMethods.tryFoundUserLang(ctx.User.Id);
        var tr = new Globalization.Using(language);
        var (ResultadoShipMsg, response) = tr.Commands.GetShipMessages(percent, user1.Username, user2.Username);

        //build
        var resultImage = await ImageModels.CreateImage.BuildShip(user1.GetAvatarUrl(ImageFormat.Png, 256), user2.GetAvatarUrl(ImageFormat.Png, 256), (byte)percent);
        using var streamImage = await ImageModels.Basics.ToStream(resultImage);

        var embed = new DiscordEmbedBuilder()
            .WithDescription($"{ResultadoShipMsg}");
        
        await ctx.RespondAsync(new DiscordMessageBuilder()
            .WithContent(response)
            .AddEmbed(embed).AddFile("file.png", streamImage));
    }
}
