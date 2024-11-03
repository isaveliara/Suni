using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;

using DSharpPlus.SlashCommands;

namespace SunSlashCommands
{
    public partial class ImageCommands : ApplicationCommandModule
    {
        [SlashCommand("ship","...")]
        public async Task SLASHCommandShip(InteractionContext ctx,
        [Option("User1","Usuário 1")] DiscordUser user2,
        [Option("User2","Usuário 2")] DiscordUser user1 = null)
        {
            user2 ??= ctx.User; if (user1 == null) user1 = ctx.User;//defaults
            
            int percent = (user1.Username + user2.Username).Where(char.IsLetter).Sum(letra => char.ToLower(letra));
            percent = (percent+50) % 100 + 1;

            string casalNome=user1.Username.Substring(0,user1.Username.Length/2)+user2.Username.Substring(user2.Username.Length/2);
            
            string ResultadoShipMsg = SunFunctions.Functions.GetShipMessage(percent, user1.Username, user2.Username);

            var resultImage = await SunImageModels.CreateImage.BuildShip(user1.GetAvatarUrl(ImageFormat.Png, 256), user2.GetAvatarUrl(ImageFormat.Png, 256), (byte)percent);
            using var streamImage = await SunImageModels.Basics.ToStream(resultImage);

            var embed = new DiscordEmbedBuilder()
                .WithDescription($"{ResultadoShipMsg}");
            //await ctx.Interaction.DeferAsync();
            await ctx.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent($":heart: | O nome do casal seria {casalNome}\n:heart: | Com uma probabilidade de {percent}")
                .AddEmbed(embed).AddFile("file.png", streamImage));
        }
    }
}