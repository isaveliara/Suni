/*using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace Sun.Dimensions.Fun
{
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
            using var connection = new SQLiteConnection("Data Source=./Suni/Functions/Dimensions/fun/andres.db; Version=3;");
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
            using var connection = new SQLiteConnection("Data Source=./Suni/Functions/Dimensions/fun/andres.db; Version=3;");
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

    public class WordAutocompleteProvider : IAutocompleteProvider
    {
        private readonly TranslationService _translationService = new TranslationService();
        public async Task<IEnumerable<DiscordAutoCompleteChoice>> Provider(AutocompleteContext ctx)
        {
            var input = ctx.FocusedOption.Value.ToString();
            var (_, suggestions) = await _translationService.GetLastWordSuggestions(input, true);

            return suggestions.Take(25).Select(suggestion => new DiscordAutoCompleteChoice(suggestion, suggestion));
        }
    }

    public class MeaningAutocompleteProvider : IAutocompleteProvider
    {
        private readonly TranslationService _translationService = new TranslationService();

        public async Task<IEnumerable<DiscordAutoCompleteChoice>> Provider(AutocompleteContext ctx)
        {
            var input = ctx.FocusedOption.Value.ToString();
            var (_, suggestions) = await _translationService.GetLastWordSuggestions(input, false);

            return suggestions.Take(25).Select(suggestion => new DiscordAutoCompleteChoice(suggestion, suggestion));
        }
    }

    public partial class FunSla : ApplicationCommandModule
    {
        [SlashCommand("word", "Sugere completions para a última palavra com base em 'word'.")]
        public async Task TranslateWordCommand(InteractionContext ctx,
            [Option("input", "Texto para completar")][Autocomplete(typeof(WordAutocompleteProvider))] string input)
        {
            var (fullTranslation, suggestions) = await new TranslationService().GetLastWordSuggestions(input, true);

            var embed = new DiscordEmbedBuilder()
                .WithTitle("Tradução e Sugestões para Word")
                .WithDescription($"Digitado:{input}\nTradução completa: {fullTranslation}")
                .WithFooter("Sugestões adicionais destacam a última palavra.")
                .WithColor(DiscordColor.Green);

            await ctx.CreateResponseAsync(embed);
        }

        [SlashCommand("meaning", "Sugere completions para a última palavra com base em 'meaning'.")]
        public async Task TranslateMeaningCommand(InteractionContext ctx,
            [Option("input", "Texto para completar")][Autocomplete(typeof(MeaningAutocompleteProvider))] string input)
        {
            var (fullTranslation, suggestions) = await new TranslationService().GetLastWordSuggestions(input, false);

            var embed = new DiscordEmbedBuilder()
                .WithTitle("Tradução e Sugestões para Meaning")
                .WithDescription($"Digitado:{input}\nTradução completa: {fullTranslation}")
                .WithFooter("Sugestões adicionais destacam a última palavra.")
                .WithColor(DiscordColor.Blue);

            await ctx.CreateResponseAsync(embed);
        }
    }
}*/