using System.Text.RegularExpressions;
using DSharpPlus.Commands.ArgumentModifiers;
using Sun.NPT.ScriptInterpreter;

namespace Sun.Commands;

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
        NptSystem parser = new NptSystem();
        var result = await parser.ParseScriptAsync(code, ctx);
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
        NptSystem parser = new NptSystem();
        var result = await parser.ParseScriptAsync(code, ctx);
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
        var (formalizedExp, _) = NPT.ScriptFormalizer.JoinScript.SetPlaceHolders(expression, ctx);
        var (diagnostic, result) = NptStatements.EvaluateExpression(formalizedExp);
        await ctx.RespondAsync($"Result of Evaluation for ``{formalizedExp}`` :\n```{result}```\nWhith Result: {diagnostic}");
    }
}