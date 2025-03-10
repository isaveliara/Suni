using Suni.Suni.Functions.Visual;
namespace Suni.Suni.Commands;

public partial class Romance
{
    [Command("ship")]
    [InteractionInstallType(DiscordApplicationIntegrationType.GuildInstall, DiscordApplicationIntegrationType.UserInstall)]
    [InteractionAllowedContexts(DiscordInteractionContextType.Guild, DiscordInteractionContextType.PrivateChannel)]
    public static async Task Ship(CommandContext ctx,
        [Parameter("user1")] DiscordUser user2,
        [Parameter("user2")] DiscordUser user1 = null)
    {
        await ctx.DeferResponseAsync();
        user2 ??= ctx.User; if (user1 == null) user1 = ctx.User;//defaults
        int percent = (user1.Username + user2.Username).Where(char.IsLetter).Sum(letra => char.ToLower(letra));
        percent = (percent+50) % 100 + 1;
        
        //translation of messages
        var solve = await SolveLang.SolveLangAsync(ctx:ctx);
        var (message, coupleName) = solve.Commands.GetShipMessages(percent, user1.Username, user2.Username);

        //build
        var resultImage = await CreateImage.BuildShip(user1.GetAvatarUrl(MediaFormat.Png, 256), user2.GetAvatarUrl(MediaFormat.Png, 256), (byte)percent);
        using var streamImage = await Basics.ToStream(resultImage);

        var embed = new DiscordEmbedBuilder()
            .WithDescription($"{message}");
        
        await ctx.RespondAsync(new DiscordMessageBuilder()
            .WithContent(coupleName)
            .AddEmbed(embed).AddFile("file.png", streamImage));
    }
}
