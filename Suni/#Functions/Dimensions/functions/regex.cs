//regex ver
//regex info
//regex 
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace Sun.Dimensions.Utilities
{
    public partial class Sla : ApplicationCommandModule
    {
        public static Dictionary<ulong, string> cache = new Dictionary<ulong, string>();

        [SlashCommandGroup("regex", "[Utilities]Regular Expression")]
        public partial class StartSlashCommandsGroup : ApplicationCommandModule
        {
            [SlashCommand("set", "Sets your RE to cache")]
            public async Task GroupRegexSLASHCommandDefine(InteractionContext ctx,
                [Option("Regex", "Regular Rxpression")] string expression)
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
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent($"The expression ``{expression}`` was cached for you! :white_check_mark:\nUse the command </regex try:123> to test them while you type!\n**Using an Exemple String:**\n``{testString}``\n\n**Results Found (first 10):**\n{matchResults}"));/////////////////////////
            }


            [SlashCommand("try","Get match groups in a Regex")]
            public async Task GroupRegexSLASHCommandTry(InteractionContext ctx,
                [Option("string", "test string")] [Autocomplete(typeof(RegexTryAutocompleteProvider))] string test)
            {
                if (!cache.TryGetValue(ctx.User.Id, out string expression)){
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Você ainda não definiu uma Regex. Use </regex set:123> primeiro."));////////////////
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

                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, 
                        new DiscordInteractionResponseBuilder().WithContent($"**Regular Expression:** `{expression}`\n**Test String:** `{test}`\n\n**Results Found (first 10):**\n{matchResults}"));
                }
                catch (Exception)
                {
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, 
                        new DiscordInteractionResponseBuilder().WithContent($":x: Unhandled error..."));
                }
            }
        }
    }

    public class RegexTryAutocompleteProvider : IAutocompleteProvider
    {
        public async Task<IEnumerable<DiscordAutoCompleteChoice>> Provider(AutocompleteContext ctx)
        {
            await Task.CompletedTask;
            if (!Sla.cache.TryGetValue(ctx.User.Id, out string expression)){
                    return new[]
                    {
                        new DiscordAutoCompleteChoice("Set u expression in /regex set", "Error")
                    };
                }

            var testString = ctx.FocusedOption.Value.ToString();
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
}