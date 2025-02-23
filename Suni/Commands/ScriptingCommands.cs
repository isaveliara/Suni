using DSharpPlus.Commands.ArgumentModifiers;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Trees.Metadata;
using Suni.Suni.NptEnvironment.Core;
using Suni.Suni.NptEnvironment.Core.Evaluator;
using Suni.Suni.NptEnvironment.Data;
using Suni.Suni.NptEnvironment.Formalizer;

namespace Suni.Suni.Commands;

[Command("scripting")]
[InteractionInstallType(DiscordApplicationIntegrationType.GuildInstall, DiscordApplicationIntegrationType.UserInstall)]
[InteractionAllowedContexts(DiscordInteractionContextType.Guild, DiscordInteractionContextType.BotDM, DiscordInteractionContextType.PrivateChannel)]
[AllowedProcessors(typeof(SlashCommandProcessor))]
public class ScriptingCommands
{
    [Command("npt")]
    [InteractionInstallType(DiscordApplicationIntegrationType.GuildInstall, DiscordApplicationIntegrationType.UserInstall)]
    [InteractionAllowedContexts(DiscordInteractionContextType.Guild, DiscordInteractionContextType.BotDM, DiscordInteractionContextType.PrivateChannel)]
    public class NptCommands
    {
        [Command("run")]
        [InteractionInstallType(DiscordApplicationIntegrationType.GuildInstall, DiscordApplicationIntegrationType.UserInstall)]
        [InteractionAllowedContexts(DiscordInteractionContextType.Guild, DiscordInteractionContextType.BotDM, DiscordInteractionContextType.PrivateChannel)]
        public static async Task NptRunCommand(CommandContext ctx, [RemainingText] string code)
        {
            if (ctx.Guild != null && !(ctx.Member?.Permissions.HasPermission(DiscordPermission.Administrator) ?? false)){
                await ctx.RespondAsync("Missing Permissions! :x:");
                return;
            }
            
            (List<string> debugs, List<string> outputs, Diagnostics result) result;
            if (code.Length > 800)
                result = (["The code Length is to Hight!"], ["..."], Diagnostics.DeniedException);
            else
                result = await Scripting.Scripting.RequestCodeExecution(0, code, ctx, Scripting.Scripting.Languages.Npt);
            
            if (result.result == Diagnostics.Forgotten){
                await ctx.DeferResponseAsync();
                return;
            }
            //building response
            string response = $"Npt Code `{SunClassBot.SuniV}`:\n```{code}```" + $"\nOutput:```\n";

            //output
            foreach (var output in result.outputs)
                response += $"\n{output}";

            if (result.result == Diagnostics.Success)
                 response += $"\n[Finished] ✅```";
            else response += $"\n[Finished] ❌```";

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

    //related to Lua
    [Command("lua")]
    [InteractionInstallType(DiscordApplicationIntegrationType.GuildInstall, DiscordApplicationIntegrationType.UserInstall)]
    [InteractionAllowedContexts(DiscordInteractionContextType.Guild, DiscordInteractionContextType.BotDM, DiscordInteractionContextType.PrivateChannel)]
    public static async Task NptRunCommand(CommandContext ctx, [RemainingText] string code)
    {
        if (ctx.Guild != null && !(ctx.Member?.Permissions.HasPermission(DiscordPermission.Administrator) ?? false)){
            await ctx.RespondAsync("Missing Permissions! :x:");
            return;
        }

        //building response
        string response = $"Lua Code:\n```lua\n{code}```" + $"\nOutput:```\n";
        
        (List<string> debugs, List<string> outputs, Diagnostics result) result;
        if (code.Length > 800)
            result = (["The code Length is to Hight!"], ["..."], Diagnostics.DeniedException);
        else
            result = await Scripting.Scripting.RequestCodeExecution(0, code, ctx, Scripting.Scripting.Languages.Lua);
        
        //output
        foreach (var output in result.outputs)
            response += $"\n{output}";

        if (result.result == Diagnostics.Success)
            response += $"\n[Finished]```";
        else response += $"\n[Finished] ❌```";

        await ctx.RespondAsync(response);
    }
}