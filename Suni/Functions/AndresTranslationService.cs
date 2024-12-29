using System.Collections.Generic;
using System.Data.SQLite;

namespace Sun.Functions;

public class AndresTranslationService
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