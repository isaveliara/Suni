using DSharpPlus.Commands.Processors.TextCommands;
using DSharpPlus.Commands.Trees.Metadata;
using Suni.Suni.NptEnvironment.Core;
using Suni.Suni.NptEnvironment.Data;

namespace Suni.Suni.Commands;

public class CustomNptCommands
{
    [Command("?")]
    [InteractionInstallType(DiscordApplicationIntegrationType.GuildInstall)]
    [InteractionAllowedContexts(DiscordInteractionContextType.Guild, DiscordInteractionContextType.BotDM, DiscordInteractionContextType.PrivateChannel)]
    [AllowedProcessors(typeof(TextCommandProcessor))]
    public static async Task About(CommandContext ctx,
        [Parameter("command")] string commandName)
    {
        var db = new DBMethods();
        var nptCommand = db.GetNptByKeyOrName(nptName: commandName, serverId: ctx.Guild.Id);
        if (nptCommand is null || nptCommand.Value.listen != "custom_command")
        {
            Console.WriteLine($"deu n");
            return;
        }
            
        NptSystem parser = new NptSystem(nptCommand.Value.nptCode, ctx);
        var result = await parser.ParseScriptAsync();

        if (result.result != Diagnostics.Success)
            await ctx.RespondAsync($"An error occurred while executing the code:\n**{result.result}**\n[Finished] :x:");

        //await ctx.RespondAsync($"executando comando {nptCommand.Value.nptName} by '{nptCommand.Value.ownerId}'\n\nscript:\n```{nptCommand.Value.nptCode}```");
    }
}