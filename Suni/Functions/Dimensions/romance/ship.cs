using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace Sun.Dimensions.Romance
{
    //prefix command

    public partial class Pre : BaseCommandModule
    {
        [Command("ship")]
        public async Task PREFIXCommandShip(CommandContext ctx,
        [Option("User1","Usuário 1")] DiscordUser user2,
        [Option("User2","Usuário 2")] DiscordUser user1 = null)
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

    //slash command

    public partial class Sla : ApplicationCommandModule
    {
        [SlashCommand("ship","Рассчитывает процент совместимости между двумя людьми")]
        public async Task SLASHCommandShip(InteractionContext ctx,
        [Option("User1","first user")] DiscordUser user2,
        [Option("User2","second user")] DiscordUser user1 = null)
        {
            await ctx.Interaction.DeferAsync();//defer
            user2 ??= ctx.User; if (user1 == null) user1 = ctx.User;//defaults
            
            int percent = (user1.Username + user2.Username).Where(char.IsLetter).Sum(letra => char.ToLower(letra));
            percent = (percent+50) % 100 + 1;

            //translation of ship messages
            var language = Functions.DB.DBMethods.tryFoundUserLang(ctx.User.Id, lang: ctx.Interaction.Locale, userName:ctx.User.Username, avatar:ctx.User.AvatarUrl);
            var tr = new Globalization.Using(language);
            var (ResultadoShipMsg, response) = tr.Commands.GetShipMessages(percent, user1.Username, user2.Username);
            
            //build
            var resultImage = await ImageModels.CreateImage.BuildShip(user1.GetAvatarUrl(ImageFormat.Png, 256), user2.GetAvatarUrl(ImageFormat.Png, 256), (byte)percent);
            using var streamImage = await ImageModels.Basics.ToStream(resultImage);

            var embed = new DiscordEmbedBuilder()
                .WithDescription($"{ResultadoShipMsg}");
            
            await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()//defer<
                .WithContent(response.ToString())
                .AddEmbed(embed).AddFile("file.png", streamImage));
        }
    }
}