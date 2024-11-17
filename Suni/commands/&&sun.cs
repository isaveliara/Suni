using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.SlashCommands;

namespace Sun.PrefixCommands
{
    public partial class Miscellaneous : BaseCommandModule
    {
        [Group("sun")] [Aliases("suni")]
        public class SunPrefixCommandsGroup : BaseCommandModule
        {
            [Command("statistics")] [Aliases("nerd")]
            public async Task PREFIXCommandStatistcs(CommandContext ctx)
            {
                var stat = Sun.Functions.Functions.GetSuniStatistics();
                await ctx.RespondAsync(stat);
            }

            [Command("inf")] [Aliases("info")]
            public async Task PREFIXCommandInfo(CommandContext ctx)
                =>
                    await ctx.RespondAsync($"Você pode ver meus detalhes em meu [website]({new Sun.Bot.DotenvItems().BaseUrl})!");
        }
    }
}
namespace Sun.SlashCommands
{
    public partial class Miscellaneous : ApplicationCommandModule
    {

        [SlashCommandGroup("suni","...")]
        public class SunSlashCommandsGroup : ApplicationCommandModule
        {
            [SlashCommand("statistics","...")] [Aliases("nerd")]
            public async Task PREFIXCommandStatistcs(InteractionContext ctx)
            {
                var stat = Sun.Functions.Functions.GetSuniStatistics();
                await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource,
                            new DSharpPlus.Entities.DiscordInteractionResponseBuilder()
                                .WithContent(stat));
            }

            [SlashCommand("informação","minhas informações"),
                NameLocalization(Localization.AmericanEnglish, "information"),
                DescriptionLocalization(Localization.AmericanEnglish, "informations about suni"),
                NameLocalization(Localization.Russian, "информация"),
                DescriptionLocalization(Localization.Russian, "моя информация")]
            public async Task PREFIXCommandInfo(InteractionContext ctx)
            {
                await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource,
                            new DSharpPlus.Entities.DiscordInteractionResponseBuilder()
                                .WithContent($"Você pode ver meus detalhes em meu [website]({new Sun.Bot.DotenvItems().BaseUrl})!"));

                System.Console.WriteLine($"the locale: {ctx.Interaction.Locale}");
                var db = new Sun.Functions.DB.DBMethods();
                db.InsertUser(ctx.User.Id, ctx.User.Username, ctx.User.AvatarUrl,
                              primaryLang: Sun.Globalization.SuniSupportedLanguages.FROM_CLIENT,
                              commandNu: 1, lastActive: System.DateTime.Now);
            }
        }
    }
}