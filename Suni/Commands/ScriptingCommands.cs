using DSharpPlus.Commands.ArgumentModifiers;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Trees.Metadata;
using Suni.Suni.NikoSharp.Core.Evaluator;
using Suni.Suni.NikoSharp.Data;
using Suni.Suni.NikoSharp.Formalizer;

namespace Suni.Suni.Commands;

[Command("scripting")]
[InteractionInstallType(DiscordApplicationIntegrationType.GuildInstall, DiscordApplicationIntegrationType.UserInstall)]
[InteractionAllowedContexts(DiscordInteractionContextType.Guild, DiscordInteractionContextType.BotDM, DiscordInteractionContextType.PrivateChannel)]
[AllowedProcessors(typeof(SlashCommandProcessor))]
public class ScriptingCommands
{
    [Command("nikosharp")]
    [InteractionInstallType(DiscordApplicationIntegrationType.GuildInstall, DiscordApplicationIntegrationType.UserInstall)]
    [InteractionAllowedContexts(DiscordInteractionContextType.Guild, DiscordInteractionContextType.BotDM, DiscordInteractionContextType.PrivateChannel)]
    public class NikoSharpCommands
    {
        [Command("run")]
        [InteractionInstallType(DiscordApplicationIntegrationType.GuildInstall, DiscordApplicationIntegrationType.UserInstall)]
        [InteractionAllowedContexts(DiscordInteractionContextType.Guild, DiscordInteractionContextType.BotDM, DiscordInteractionContextType.PrivateChannel)]
        public static async Task NikoSharpRunCommand(CommandContext ctx, [RemainingText] string code)
        {
            if (ctx.Guild != null && !(ctx.Member?.Permissions.HasPermission(DiscordPermission.Administrator) ?? false)){
                await ctx.RespondAsync("Missing Permissions! :x:");
                return;
            }
            
            (List<string> debugs, List<string> outputs, Diagnostics result) result;
            if (code.Length > 800)
                result = (["The code Length is to Hight!"], ["..."], Diagnostics.RaisedException);
            else
                result = await Scripting.Scripting.RequestCodeExecution(0, code, ctx, Scripting.Scripting.Languages.NikoSharp);
            
            if (result.result == Diagnostics.Forgotten){
                await ctx.DeferResponseAsync();
                return;
            }
            //building response
            string response = $"NikoSharp `{SunClassBot.SuniV}`:\n```{code}```" + $"\nOutput:```\n";

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
        public static async Task NikoSharpEvaluateCommand(CommandContext ctx, [RemainingText] string expression)
        {
            FormalizingScript formalizingScript = new FormalizingScript(expression, ctx);
            EnvironmentDataContext data = formalizingScript.GetFormalized;
            string formalizedExpression = data.Lines[0];

            var (result, diagnostic, msgEvaluation) = NikoSharpEvaluator.EvaluateExpression(formalizedExpression, data);
            string resultStr = result is not null? $"```{result}```" : "";

            await ctx.RespondAsync(($"Result of Evaluation for ``{formalizedExpression}`` :\n{resultStr}\nWhith Result: ```{diagnostic} " + msgEvaluation?? "") + "```");
        }
    }

    //related to Lua
    [Command("lua")]
    [InteractionInstallType(DiscordApplicationIntegrationType.GuildInstall, DiscordApplicationIntegrationType.UserInstall)]
    [InteractionAllowedContexts(DiscordInteractionContextType.Guild, DiscordInteractionContextType.BotDM, DiscordInteractionContextType.PrivateChannel)]
    public static async Task LuaRunCommand(CommandContext ctx, [RemainingText] string code)
    {
        if (ctx.Guild != null && !(ctx.Member?.Permissions.HasPermission(DiscordPermission.Administrator) ?? false)){
            await ctx.RespondAsync("Missing Permissions! :x:");
            return;
        }

        //building response
        string response = $"Lua Code:\n```lua\n{code}```" + $"\nOutput:```\n";
        
        (List<string> debugs, List<string> outputs, Diagnostics result) result;
        if (code.Length > 800)
            result = (["The code Length is to Hight!"], ["..."], Diagnostics.RaisedException);
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