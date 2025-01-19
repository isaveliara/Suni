using System.Collections.Generic;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Commands.Trees;
namespace Suni.Suni.Translations;

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
