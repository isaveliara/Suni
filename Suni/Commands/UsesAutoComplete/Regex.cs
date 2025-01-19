using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;

namespace Suni.Suni.Commands.UsesAutoComplete;
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
        var solved = await SolveLang.SolveLangAsync(ctx:ctx);

        cache[ctx.User.Id] = expression;
        string testString = "I_Wanna_Test**This**123_ABC-def-456 GHIJKL @regex.test#match! 2024-11-15 email@example.com (captura) [grupos] {123}";
        string matchResults;
        try{
            var regex = new Regex(expression);
            var matches = regex.Matches(testString);

            if (matches.Count > 0){
                var matchList = matches.Cast<Match>().Take(10).Select((m, i) => $"- [{i + 1}] {m.Value}");
                matchResults = string.Join("\n", matchList);
            }
            else
                matchResults = solved.GenericMessages.ErrNoResources;
        }
        catch (Exception){
            matchResults = solved.GenericMessages.ErrUnknown;
        }
        await ctx.RespondAsync($"The expression ``{expression}`` was cached for you! :white_check_mark:\nUse the command </regex try:1322000293776588800> to test them while you type!\n**Using an Exemple String:**\n``{testString}``\n\n**Results Found (first 10):**\n{matchResults}");
    }


    [Command("try")]
    [InteractionInstallType(DiscordApplicationIntegrationType.GuildInstall, DiscordApplicationIntegrationType.UserInstall)]
    [InteractionAllowedContexts(DiscordInteractionContextType.Guild, DiscordInteractionContextType.BotDM, DiscordInteractionContextType.PrivateChannel)]
    public async Task Try(CommandContext ctx,
        [Parameter("test_string")] [SlashAutoCompleteProvider(typeof(RegexTryAutocompleteProvider))] string test)
    {
        var solved = await SolveLang.SolveLangAsync(ctx:ctx);

        if (!cache.TryGetValue(ctx.User.Id, out string expression)){
            await ctx.RespondAsync("Você ainda não definiu uma Regex. Use </regex define:1322000293776588800> antes de executar esse comando.");
            return;
        }

        try{
            var regex = new Regex(expression);
            var matches = regex.Matches(test);
            var embed = new DiscordEmbedBuilder()
                .WithTitle(expression)
                .WithColor(DiscordColor.IndianRed)
                .WithDescription($"String: `{test}`\n\\* The first 10 results will be taken.");
            
            
            if (matches.Count > 0){
                var matchList = matches.Cast<Match>().Take(10).Select((m, i) => $"- [{i + 1}] {m.Value}");
                //add the fields
                foreach (var (match, index) in matches.Cast<Match>().Take(10).Select((m, i) => (m, i + 1)))
                    embed.AddField($"Match [{index}]", $"[{match.Value}]", inline: false);

                await ctx.RespondAsync(embed);
                return;
            }
            
            await ctx.RespondAsync(content: solved.GenericMessages.ErrNoResources, embed: embed);
        }
        catch (Exception){
            await ctx.RespondAsync(solved.GenericMessages.ErrInternal);
        }
    }

    [Command("automod")]
    [Description("Tests whether a string will be marked by AutoMod.")]
    [RequirePermissions(DiscordPermission.Administrator & DiscordPermission.ManageGuild)]
    [InteractionInstallType(DiscordApplicationIntegrationType.GuildInstall)]
    [InteractionAllowedContexts(DiscordInteractionContextType.Guild)]
    public async Task AutomodDenies(CommandContext ctx,
        [Parameter("test_string")] string testString)
    {
        var solved = await SolveLang.SolveLangAsync(ctx:ctx);

        if (!ctx.Member.Permissions.HasPermission(DiscordPermission.Administrator & DiscordPermission.ManageGuild)){
            await ctx.RespondAsync(solved.GenericMessages.ErrUnauthorized);
            return;
        }

        IReadOnlyList<DiscordAutoModerationRule> automodRules = await ctx.Guild.GetAutoModerationRulesAsync();

        if (automodRules is null || automodRules.Count == 0){
            await ctx.RespondAsync(solved.GenericMessages.ErrNoResources);
            return;
        }
        var triggeredRules = new List<string>();
        foreach (var automodRule in automodRules)
        {
            string ruleName = automodRule.Name;
            IReadOnlyList<string> ruleKeywords = automodRule.Metadata.KeywordFilter;
            IReadOnlyList<string> ruleRegex = automodRule.Metadata.RegexPatterns;

            //check keywords
            if (ruleKeywords != null && ruleKeywords.Count > 0)
                foreach (string keyword in ruleKeywords){
                    if (MatchesKeyword(testString, keyword)){
                        triggeredRules.Add(ruleName);
                        break;
                    }
                }

            //check regular expressions
            if (ruleRegex != null && ruleRegex.Count > 0)
                foreach (string pattern in ruleRegex){
                    if (Regex.IsMatch(testString, pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline)){
                        triggeredRules.Add(ruleName);
                        break;
                    }
                }
        }

        //return results
        if (triggeredRules.Count > 0){
            string response = solved.Commands.GetString("automodFlaggedText") +
                            $"\n{string.Join("\n", triggeredRules.Select(rule => $"- {rule}"))}";
            await ctx.RespondAsync(response);
            return;
        }
        await ctx.RespondAsync(solved.Commands.GetString("automodUnflaggedText"));
    }

    //check if a keyword matches the text
    private bool MatchesKeyword(string testString, string keyword)
    {
        if (keyword.StartsWith("*") && keyword.EndsWith("*")){
            // *ana* ⇒ match in any position
            string substring = keyword.Trim('*');
            return testString.Contains(substring, StringComparison.OrdinalIgnoreCase);
        }
        else if (keyword.StartsWith("*")){
            // *cat ⇒ match at the end of the word
            string suffix = keyword.TrimStart('*');
            return testString.EndsWith(suffix, StringComparison.OrdinalIgnoreCase);
        }
        else if (keyword.EndsWith("*")){
            // cat* ⇒ match at the beginning of the word
            string prefix = keyword.TrimEnd('*');
            return testString.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
        }
        else
            //full word (exact match)
            return testString.Equals(keyword, StringComparison.OrdinalIgnoreCase);
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
