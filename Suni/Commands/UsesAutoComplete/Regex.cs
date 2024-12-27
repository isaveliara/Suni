using System.Collections.Generic;
using System.Text.RegularExpressions;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;

namespace Sun.Commands;
[Command("regex")]
[InteractionInstallType(DiscordApplicationIntegrationType.GuildInstall, DiscordApplicationIntegrationType.UserInstall)]
[InteractionAllowedContexts(DiscordInteractionContextType.Guild, DiscordInteractionContextType.BotDM, DiscordInteractionContextType.PrivateChannel)]
public partial class RegexCommandsGroup
{
    public static Dictionary<ulong, string> cache = new Dictionary<ulong, string>();

    [Command("define")]
    [InteractionInstallType(DiscordApplicationIntegrationType.GuildInstall, DiscordApplicationIntegrationType.UserInstall)]
    [InteractionAllowedContexts(DiscordInteractionContextType.Guild, DiscordInteractionContextType.BotDM, DiscordInteractionContextType.PrivateChannel)]
    public async Task Define(CommandContext ctx,
        [Parameter("regex")] string expression)
    {
        cache[ctx.User.Id] = expression;
        string testString = "I_Wanna_Test**This**123_ABC-def-456 GHIJKL @regex.test#match! 2024-11-15 email@example.com (captura) [grupos] {123}";
        string matchResults;
        try
        {
            var regex = new Regex(expression);
            var matches = regex.Matches(testString);

            if (matches.Count > 0)
            {
                var matchList = matches.Cast<Match>().Take(10).Select((m, i) => $"- [{i + 1}] {m.Value}");
                matchResults = string.Join("\n", matchList);
            }
            else
                matchResults = "No Matchs Found.";
        }
        catch (Exception){
            matchResults = $":x: | Error while trying your regex.";
        }
        await ctx.RespondAsync($"The expression ``{expression}`` was cached for you! :white_check_mark:\nUse the command </regex try:123> to test them while you type!\n**Using an Exemple String:**\n``{testString}``\n\n**Results Found (first 10):**\n{matchResults}");
    }


    [Command("try")]
    [InteractionInstallType(DiscordApplicationIntegrationType.GuildInstall, DiscordApplicationIntegrationType.UserInstall)]
    [InteractionAllowedContexts(DiscordInteractionContextType.Guild, DiscordInteractionContextType.BotDM, DiscordInteractionContextType.PrivateChannel)]
    public async Task Try(CommandContext ctx,
        [Parameter("test_string")] [SlashAutoCompleteProvider(typeof(RegexTryAutocompleteProvider))] string test)
    {
        if (!cache.TryGetValue(ctx.User.Id, out string expression)){
            await ctx.RespondAsync("Você ainda não definiu uma Regex. Use </regex set:123> primeiro.");
            return;
        }

        string matchResults;
        try
        {
            var regex = new Regex(expression);
            var matches = regex.Matches(test);

            string result = $"**Regular Expression:** `{expression}`\n**Matches:** {matches.Count}";
            if (matches.Count > 0)
            {
                var matchList = matches.Cast<Match>().Take(10).Select((m, i) => $"- [{i + 1}] {m.Value}");
                matchResults = string.Join("\n", matchList);
            }
            else matchResults = "No matches Found.";

            await ctx.RespondAsync($"**Regular Expression:** `{expression}`\n**Test String:** `{test}`\n\n**Results Found (first 10):**\n{matchResults}");
        }
        catch (Exception)
        {
            await ctx.RespondAsync($":x: Unhandled error...");
        }
    }

}

public class RegexTryAutocompleteProvider : IAutoCompleteProvider
{
    public async ValueTask<IEnumerable<DiscordAutoCompleteChoice>> AutoCompleteAsync(AutoCompleteContext ctx)
    {
        await Task.CompletedTask;
        if (!RegexCommandsGroup.cache.TryGetValue(ctx.User.Id, out string expression)){
                return new[]
                {
                    new DiscordAutoCompleteChoice("Set u expression in /regex set", "Error")
                };
            }

        var testString = ctx.UserInput; //.FocusedOption.Value.ToString();
        Console.WriteLine(expression);

        if (string.IsNullOrWhiteSpace(testString))
        {
            return new[]
            {
                new DiscordAutoCompleteChoice("Fill the Expression!", "Error")
            };
        }

        try
        {
            var regex = new Regex(expression);

            var matches = regex.Matches(testString);
            var matchCount = matches.Count;
            var firstMatch = matches.Count > 0 ? matches[0].Value : "No matches Found.";

            return new[]
            {
                new DiscordAutoCompleteChoice($"({matchCount} matchs) {firstMatch}", expression)
            };
        }
        catch (Exception)
        {
            return new[]
            {
                new DiscordAutoCompleteChoice("A Regex Error occurred", $"Error")
            };
        }
    }
}
