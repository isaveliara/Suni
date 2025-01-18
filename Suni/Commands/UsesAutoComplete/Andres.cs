using System.Collections.Generic;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Commands.Processors.UserCommands;
using DSharpPlus.Commands.Trees.Metadata;

namespace Suni.Suni.Commands.UsesAutoComplete;

[Command("andres")]
[InteractionInstallType(DiscordApplicationIntegrationType.GuildInstall, DiscordApplicationIntegrationType.UserInstall)]
[InteractionAllowedContexts(DiscordInteractionContextType.Guild, DiscordInteractionContextType.BotDM, DiscordInteractionContextType.PrivateChannel)]
public partial class AndresCommandsGroup
{
    [Command("word")]
    [AllowedProcessors(typeof(UserCommandProcessor))]
    [SlashCommandTypes(DiscordApplicationCommandType.SlashCommand)]
    [InteractionInstallType(DiscordApplicationIntegrationType.GuildInstall, DiscordApplicationIntegrationType.UserInstall)]
    [InteractionAllowedContexts(DiscordInteractionContextType.Guild, DiscordInteractionContextType.BotDM, DiscordInteractionContextType.PrivateChannel)]
    public async Task Word(CommandContext ctx,
        [Parameter("input")] [SlashAutoCompleteProvider(typeof(AndresTranslationWordAutocompleteProvider))] string input)
    {
        var (fullTranslation, suggestions) = await new AndresTranslationService().GetLastWordSuggestions(input, true);
        var embed = new DiscordEmbedBuilder()
            .WithTitle("Tradução e Sugestões para Word")
            .WithDescription($"Digitado:{input}\nTradução completa: {fullTranslation}")
            .WithFooter("Sugestões adicionais destacam a última palavra.")
            .WithColor(DiscordColor.Green);

        await ctx.RespondAsync(embed);
    }


    [Command("meaning")]
    [AllowedProcessors(typeof(UserCommandProcessor))]
    [SlashCommandTypes(DiscordApplicationCommandType.SlashCommand)]
    [InteractionInstallType(DiscordApplicationIntegrationType.GuildInstall, DiscordApplicationIntegrationType.UserInstall)]
    [InteractionAllowedContexts(DiscordInteractionContextType.Guild, DiscordInteractionContextType.BotDM, DiscordInteractionContextType.PrivateChannel)]
    public async Task Meaning(CommandContext ctx,
        [Parameter("input")] [SlashAutoCompleteProvider(typeof(AndresTranslationMeaningAutocompleteProvider))] string input)
    {
        var (fullTranslation, suggestions) = await new AndresTranslationService().GetLastWordSuggestions(input, false);
        var embed = new DiscordEmbedBuilder()
            .WithTitle("Tradução e Sugestões para Meaning")
            .WithDescription($"Digitado:{input}\nTradução completa: {fullTranslation}")
            .WithFooter("Sugestões adicionais destacam a última palavra.")
            .WithColor(DiscordColor.Blue);

        await ctx.RespondAsync(embed);
    }
}

public class AndresTranslationWordAutocompleteProvider : IAutoCompleteProvider
{
    private readonly AndresTranslationService _translationService = new AndresTranslationService();
    public async ValueTask<IEnumerable<DiscordAutoCompleteChoice>> AutoCompleteAsync(AutoCompleteContext ctx)
    {
        var input = ctx.UserInput;
        var (_, suggestions) = await _translationService.GetLastWordSuggestions(input, true);

        return suggestions.Take(25).Select(suggestion => new DiscordAutoCompleteChoice(suggestion, suggestion));
    }
}
public class AndresTranslationMeaningAutocompleteProvider : IAutoCompleteProvider
{
    private readonly AndresTranslationService _translationService = new AndresTranslationService();
    public async ValueTask<IEnumerable<DiscordAutoCompleteChoice>> AutoCompleteAsync(AutoCompleteContext ctx)
    {
        var input = ctx.UserInput;
        var (_, suggestions) = await _translationService.GetLastWordSuggestions(input, false);

        return suggestions.Take(25).Select(suggestion => new DiscordAutoCompleteChoice(suggestion, suggestion));
    }
}
