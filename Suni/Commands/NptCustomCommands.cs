using DSharpPlus.Commands.Processors.TextCommands;
using DSharpPlus.Commands.Trees.Metadata;
using Sun.NptEnvironment.Core;
using Sun.NptEnvironment.Data;

namespace Sun.Commands;

public class CustomNptCommands
{
    [Command("?")]
    [InteractionInstallType(DiscordApplicationIntegrationType.GuildInstall)]
    [InteractionAllowedContexts(DiscordInteractionContextType.Guild, DiscordInteractionContextType.BotDM, DiscordInteractionContextType.PrivateChannel)]
    [AllowedProcessors(typeof(TextCommandProcessor))]
    public static async Task CustomCommandCaller(CommandContext ctx,
        [Parameter("command")] string commandName)
    {
        var db = new DBMethods();
        var nptCommand = db.GetNptByKeyOrName(nptName: commandName, serverId: ctx.Guild.Id);
        if (nptCommand is null) //|| nptCommand.Value.listen != "custom_command")
        {
            Console.WriteLine("jucdn");
            return;
        }
            
        NptSystem parser = new NptSystem();
        var result = await parser.ParseScriptAsync(nptCommand.Value.nptCode, ctx);

        if (result.result != Diagnostics.Success)
            await ctx.RespondAsync($"An error occurred while executing the code:\n**{result.result}**\n[Finished] :x:");

        await ctx.RespondAsync($"executando comando {nptCommand.Value.nptName} by '{nptCommand.Value.ownerId}'\n\nscript:\n```{nptCommand.Value.nptCode}```");
    }
}