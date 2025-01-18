using System.Collections.Generic;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Commands.Trees;

namespace Suni.Suni.Commands
{
    public class LanguageCommands
    {
        [Command("language")]
        [InteractionInstallType(DiscordApplicationIntegrationType.GuildInstall, DiscordApplicationIntegrationType.UserInstall)]
        [InteractionAllowedContexts(DiscordInteractionContextType.Guild, DiscordInteractionContextType.PrivateChannel)]
        public async Task SetLanguage(CommandContext ctx,
            [SlashChoiceProvider(typeof(LanguagesChoicesProvider))] [Parameter("language")] string language)
        {
            //translation of messages
            var solve = await SolveLang.SolveLangAsync(ctx:ctx);
            
            string message;

            var success = await new DBMethods().UpdateUserPrimaryLangAsync(ctx.User.Id, GlobalizationMethods.ParseToLanguageSupported(language)); //.SetUserLang(ctx.User.Id, language);
            if (!success)
            {
                message = solve.Commands.GetString("SetLangF");
                await ctx.RespondAsync(string.Format(message, language));
                return;
            }
            solve = await SolveLang.SolveLangAsync(language);

            //success on edit language
            var coverage = solve.Commands.GetTranslationCoveragePercentage();
            message = solve.Commands.GetString("SetLang");

            await ctx.RespondAsync(string.Format(message, language, string.Format("{0:N}", coverage)));
        }
    }
}

namespace Suni.Suni.Globalization{
    public class LanguagesChoicesProvider : IChoiceProvider
    {
        private static readonly IReadOnlyList<DiscordApplicationCommandOptionChoice> Languages =
        [
            new DiscordApplicationCommandOptionChoice("🇧🇷 Português", "PT"),
            new DiscordApplicationCommandOptionChoice("🇺🇸 English", "EN"),
            new DiscordApplicationCommandOptionChoice("🇷🇺 Русский", "RU"),
            new DiscordApplicationCommandOptionChoice("🇲🇽 Español", "ES-MX")
        ];

        public async ValueTask<IEnumerable<DiscordApplicationCommandOptionChoice>> ProvideAsync(CommandParameter parameter)
        {
            await Task.CompletedTask;
            return Languages;
        }
    }
}