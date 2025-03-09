using DSharpPlus.Commands.Processors.TextCommands;
using DSharpPlus.Commands.Trees.Metadata;
using Suni.Suni.NikoSharp.Core;
using Suni.Suni.NikoSharp.Data;
namespace Suni.Suni.Commands;

public class CustomCommands
{
    [Command("?")]
    [InteractionInstallType(DiscordApplicationIntegrationType.GuildInstall)]
    [InteractionAllowedContexts(DiscordInteractionContextType.Guild, DiscordInteractionContextType.BotDM, DiscordInteractionContextType.PrivateChannel)]
    [AllowedProcessors(typeof(TextCommandProcessor))]
    public static async Task About(CommandContext ctx,
        [Parameter("command")] string commandName)
    {
        var db = new DBMethods();
        var nikosharpCommand = db.GetScriptByKeyOrName(nikosharpName: commandName, serverId: ctx.Guild.Id);
        if (nikosharpCommand is null || nikosharpCommand.Value.listen != "custom_command"){
            Console.WriteLine($"deu n");
            return;
        }
            
        var parser = new NikoSharpSystem(nikosharpCommand.Value.nikosharpCode, ctx);
        var result = await parser.ParseScriptAsync();

        if (result.result != Diagnostics.Success)
            await ctx.RespondAsync($"An error occurred while executing the code:\n**{result.result}**\n[Finished] :x:");
    }
}