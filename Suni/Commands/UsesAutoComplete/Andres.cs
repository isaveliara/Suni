using System.Collections.Generic;
using System.Data.SQLite;
using System.Text.RegularExpressions;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Commands.Processors.UserCommands;
using DSharpPlus.Commands.Trees.Metadata;

namespace Sun.Commands;

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
        var (fullTranslation, suggestions) = await new TranslationService().GetLastWordSuggestions(input, true);
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
        var (fullTranslation, suggestions) = await new TranslationService().GetLastWordSuggestions(input, false);
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
    private readonly TranslationService _translationService = new TranslationService();
    public async ValueTask<IEnumerable<DiscordAutoCompleteChoice>> AutoCompleteAsync(AutoCompleteContext ctx)
    {
        var input = ctx.UserInput;
        var (_, suggestions) = await _translationService.GetLastWordSuggestions(input, true);

        return suggestions.Take(25).Select(suggestion => new DiscordAutoCompleteChoice(suggestion, suggestion));
    }
}
public class AndresTranslationMeaningAutocompleteProvider : IAutoCompleteProvider
{
    private readonly TranslationService _translationService = new TranslationService();
    public async ValueTask<IEnumerable<DiscordAutoCompleteChoice>> AutoCompleteAsync(AutoCompleteContext ctx)
    {
        var input = ctx.UserInput;
        var (_, suggestions) = await _translationService.GetLastWordSuggestions(input, false);

        return suggestions.Take(25).Select(suggestion => new DiscordAutoCompleteChoice(suggestion, suggestion));
    }
}

public class TranslationService
{
    public async Task<(string fullTranslation, List<string> suggestions)> GetLastWordSuggestions(string input, bool isWordSearch)
    {
        var words = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (words.Length == 0)
            return ("?palavra", new List<string>());

        //translate the phrase
        var translatedPhrase = new List<string>();
        foreach (var word in words)
        {
            var normalizedWord = NormalizeWord(word);
            var result = await QueryTranslation(normalizedWord, isWordSearch);

            translatedPhrase.Add(result ?? $"?{word}");
        }

        var fullTranslation = string.Join(" ", translatedPhrase);

        //complete the last word
        var lastWord = words.Last();
        var normalizedLastWord = NormalizeWord(lastWord);
        var completions = await QueryCompletions(normalizedLastWord, isWordSearch);

        var suggestions = completions.Select(completion =>
        {
            var remaining = completion.Substring(normalizedLastWord.Length);
            return input.Replace(lastWord, $"{lastWord}{remaining}");
        }).ToList();

        if (!suggestions.Any())
            suggestions.Add($"?{lastWord}");

        return (fullTranslation, suggestions);
    }

    private async Task<string> QueryTranslation(string word, bool isWordSearch)
    {
        using var connection = new SQLiteConnection("Data Source=./Suni/Commands/andres.db; Version=3;");
        await connection.OpenAsync();

        var column = isWordSearch ? "word" : "meaning";
        var query = $"SELECT {(isWordSearch ? "meaning" : "word")} FROM words WHERE LOWER({column}) = LOWER(@word) LIMIT 1";
        var command = new SQLiteCommand(query, connection);
        command.Parameters.AddWithValue("@word", word);

        var result = await command.ExecuteScalarAsync();
        return result?.ToString();
    }

    private async Task<List<string>> QueryCompletions(string word, bool isWordSearch)
    {
        using var connection = new SQLiteConnection("Data Source=./Suni/Commands/andres.db; Version=3;");
        await connection.OpenAsync();

        var column = isWordSearch ? "word" : "meaning";
        var query = $"SELECT DISTINCT {column} FROM words WHERE LOWER({column}) LIKE LOWER(@word || '%') LIMIT 25";
        var command = new SQLiteCommand(query, connection);
        command.Parameters.AddWithValue("@word", word);

        var results = new List<string>();
        using (var reader = await command.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
                results.Add(reader.GetString(0));
        }

        return results;
    }

    private string NormalizeWord(string word){
        var allowedAccents = "áàãâéêíóôõúüçÁÀÃÂÉÊÍÓÔÕÚÜÇ";
        word = word.ToLower();
        return new string(word.Where(c => char.IsLetter(c) || allowedAccents.Contains(c)).ToArray());
    }
}