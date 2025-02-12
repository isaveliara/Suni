using System.Text.RegularExpressions;
using DSharpPlus.Commands.ArgumentModifiers;
using Suni.Suni.NptEnvironment.Core;
using Suni.Suni.NptEnvironment.Core.Evaluator;
using Suni.Suni.NptEnvironment.Data;
using Suni.Suni.NptEnvironment.Formalizer;

namespace Suni.Suni.Commands;

[Command("npt")]
[InteractionInstallType(DiscordApplicationIntegrationType.GuildInstall, DiscordApplicationIntegrationType.UserInstall)]
[InteractionAllowedContexts(DiscordInteractionContextType.Guild, DiscordInteractionContextType.BotDM, DiscordInteractionContextType.PrivateChannel)]
public class NptCommands
{
    [Command("debug")]
    [InteractionInstallType(DiscordApplicationIntegrationType.GuildInstall, DiscordApplicationIntegrationType.UserInstall)]
    [InteractionAllowedContexts(DiscordInteractionContextType.Guild, DiscordInteractionContextType.BotDM, DiscordInteractionContextType.PrivateChannel)]
    public static async Task NptRunDebuggingCommand(CommandContext ctx, [RemainingText] string msg)
    {
        if (ctx.Guild == null && !ctx.Member.Permissions.HasPermission(DiscordPermission.Administrator))
        {
            await ctx.RespondAsync("Voçê não pode executar isso.");
            return;
        }

        string code;
        Match match = Regex.Match(msg, @"```(.+?)```", RegexOptions.Singleline);
        if (!match.Success)
        {
            await ctx.RespondAsync("Falha ao identificar o script.");
            return;
        }
        code = match.Groups[1].Value;
        Console.WriteLine(code);

        //building response
        string response = $"Result (Debugging) of SuniNPT code `{SunClassBot.SuniV}` is here:```\n";
        NptSystem parser = new NptSystem(code, ctx);
        var result = await parser.ParseScriptAsync();
        if (result.result == Diagnostics.Forgotten)
            return;
        //first output
        foreach (var output in result.outputs)
            response += $"\n{output}";
        response += "\n\nDEBUG:\n";

        //debug
        foreach (var debug in result.debugs)
            response += $"\n    {debug}";

        if (result.result == Diagnostics.Success)
            response += $"```\nResult Program: **{result.result}**\n[Finished] :white_check_mark:";
        else response += $"```\nOcorreu um erro ao executar o código:\n**{result.result}**\n[Finished] :x:";

        await ctx.RespondAsync(response);
    }

    [Command("run")]
    [InteractionInstallType(DiscordApplicationIntegrationType.GuildInstall, DiscordApplicationIntegrationType.UserInstall)]
    [InteractionAllowedContexts(DiscordInteractionContextType.Guild, DiscordInteractionContextType.BotDM, DiscordInteractionContextType.PrivateChannel)]
    public static async Task NptRunCommand(CommandContext ctx, [RemainingText] string msg)
    {
        if (ctx.Guild != null && !(ctx.Member?.Permissions.HasPermission(DiscordPermission.Administrator) ?? false))
        {
            await ctx.RespondAsync("Voçê não pode executar isso.");
            return;
        }

        string code;
        Match match = Regex.Match(msg, @"```(.+?)```", RegexOptions.Singleline);
        if (!match.Success)
        {
            await ctx.RespondAsync("Falha ao identificar o script.");
            return;
        }
        code = match.Groups[1].Value;
        Console.WriteLine(code);

        //building response
        string response = $"OUTPUT of SuniNPT code `{SunClassBot.SuniV}` is here:```\n";
        NptSystem parser = new NptSystem(code, ctx);
        var result = await parser.ParseScriptAsync();
        if (result.result == Diagnostics.Forgotten)
            return;
        //output
        foreach (var output in result.outputs)
            response += $"\n{output}";

        if (result.result == Diagnostics.Success)
            response += $"```\nResult Program: **{result.result}**\n[Finished] :white_check_mark:";
        else response += $"```\nOcorreu um erro ao executar o código:\n**{result.result}**\n[Finished] :x:";

        await ctx.RespondAsync(response);
    }

    [Command("evaluate")]
    [InteractionInstallType(DiscordApplicationIntegrationType.GuildInstall, DiscordApplicationIntegrationType.UserInstall)]
    [InteractionAllowedContexts(DiscordInteractionContextType.Guild, DiscordInteractionContextType.BotDM, DiscordInteractionContextType.PrivateChannel)]
    public static async Task NptEvaluateCommand(CommandContext ctx, [RemainingText] string expression)
    {
        FormalizingScript formalizingScript = new FormalizingScript(expression, ctx);
        EnvironmentDataContext data = formalizingScript.GetFormalized;
        string formalizedExpression = data.Lines[0];

        var (result, diagnostic, msgEvaluation) = NptEvaluator.EvaluateExpression(formalizedExpression, data);
        string resultStr = result is not null? $"```{result}```" : "";

        await ctx.RespondAsync(($"Result of Evaluation for ``{formalizedExpression}`` :\n{resultStr}\nWhith Result: ```{diagnostic} " + msgEvaluation?? "") + "```");
    }
}