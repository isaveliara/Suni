using System.Collections.Generic;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Commands.Trees;
using Sun.Functions.DB;

namespace Sun.Commands
{
    public class LanguageCommands
    {
        [Command("language")]
        [InteractionInstallType(DiscordApplicationIntegrationType.GuildInstall, DiscordApplicationIntegrationType.UserInstall)]
        [InteractionAllowedContexts(DiscordInteractionContextType.Guild, DiscordInteractionContextType.PrivateChannel)]
        public async Task SetLanguage(CommandContext ctx,
            [SlashChoiceProvider(typeof(Sun.Globalization.LanguagesChoicesProvider))] [Parameter("language")] string language)
        {
            var success = DBMethods.SetUserLang(ctx.User.Id, language);
            if (!success)
            {
                await ctx.RespondAsync($"Falha ao definir o idioma para '{language}'!");
                return;
            }
            await ctx.RespondAsync($"idioma definido para {language}");
        }
    }
}

namespace Sun.Globalization{
    public class LanguagesChoicesProvider : IChoiceProvider
    {
        private static readonly IReadOnlyList<DiscordApplicationCommandOptionChoice> Languages =
        [
            new DiscordApplicationCommandOptionChoice("Português", "PT"),
            new DiscordApplicationCommandOptionChoice("English", "EN"),
            new DiscordApplicationCommandOptionChoice("Русский", "RU"),
        ];

        public async ValueTask<IEnumerable<DiscordApplicationCommandOptionChoice>> ProvideAsync(CommandParameter parameter)
            => Languages;
    }
}