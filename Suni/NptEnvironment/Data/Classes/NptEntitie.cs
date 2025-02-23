using Suni.Suni.NptEnvironment.Data.Types;
namespace Suni.Suni.NptEnvironment.Data.Classes;

public static partial class NptEntitie
{
    public static List<string> LibMethods { get; } = new List<string> { "log", "ban", "unban", "react", "respond" };

    /// <summary>
    /// Controler of npt class.
    /// </summary>
    /// <param name="methodName"></param>
    /// <param name="args"></param>
    /// <param name="ctx"></param>
    /// <returns></returns>
    public static async Task<Diagnostics> Controler(string methodName, NptGroup args, CommandContext ctx)
    {
        //set a try-catch here for controller of args
        Diagnostics result;
        try{
            switch (methodName)
            {
                case "log": //npt::log(s'My Message') -> 1234567891011121314
                    ulong argChannelId = (ulong)args.Pointer().Value;
                    string argContentMessage = string.Join('\0', args);
                    result = await NptEntitie.Log(ctx, argChannelId, argContentMessage);
                    break;
                case "respond":
                    result = await NptEntitie.Respond(ctx, args);
                    break;
                case "react": //npt::react(s':x:') -> <message id>
                    ulong argMessageId = (ulong)args.Pointer().Value;
                    string argReactionId = args.FirstValue().Value.ToString();
                    result = await NptEntitie.React(ctx, argMessageId, argReactionId);
                    break;
                case "ban": //npt::ban(s'You broke a rule!') -> <user id>
                    ulong userId = (ulong)args.Pointer().Value;
                    result = await Ban(ctx, userId, args.FirstValue().Value.ToString());
                    break;
                case "unban": //npt::unban(s'Sorry!') -> <user id>
                    result = await Unban(ctx, (ulong)args.Pointer().Value, args.FirstValue().Value.ToString());
                    break;
                default: //npt::invalidmethod() -> nil
                    result = Diagnostics.NotFoundIncludedObjectException;
                    break;
            }
        }
        catch (Exception ex){
            Console.WriteLine(ex);
            result = Diagnostics.UnknowException;
        }
        return result;
    }

    private static async Task<Diagnostics> Respond(CommandContext ctx, NptGroup args)
    {
        try{
            if (args.FirstValue().Value is null)
                return Diagnostics.ArgumentMismatch;
            
            //pointer will be something like -> embed
            DiscordMessageBuilder message;
            if (args.Pointer().Value.ToString() == "embedded")
            {
                DiscordColor discordColor = new DiscordColor(new Random().Next(0, 16777215));
                message = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithDescription(args.FirstValue().Value.ToString())
                        .WithColor(discordColor));
            }
            else{
                message = new DiscordMessageBuilder().WithContent(args.FirstValue().Value.ToString());
            }

            if (string.IsNullOrEmpty(ctx.User.Locale)) //if this value is null, means that this is a application command, and just send as a normal log &{channelId}
                 await ctx.Channel.SendMessageAsync(message);
            else await ctx.RespondAsync(message);

            return Diagnostics.Success;
        }
        catch (Exception){
            return Diagnostics.DeniedException;
        }
    }

    private static async Task<Diagnostics> Ban(CommandContext ctx, ulong userId, string reason)
    {
        try{
            var user = await ctx.Guild.GetMemberAsync(userId);
            if (user == null)
                return Diagnostics.NPTInvalidUserException;

            await ctx.Guild.BanMemberAsync(await ctx.Guild.GetMemberAsync(userId), reason:reason);
            return Diagnostics.Success;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return Diagnostics.NPTMissingPermissionsException;
        }
    }

    private static async Task<Diagnostics> Log(CommandContext ctx, ulong channelId, string message){
        try{
            if (!ctx.Guild.Channels.ContainsKey(channelId))
                return Diagnostics.NPTInvalidChannelException;
            
            var channel = await ctx.Client.GetChannelAsync(channelId);
            await channel.SendMessageAsync(message); //sends
            return Diagnostics.Success;
        }
        catch (Exception){
            return Diagnostics.NPTMissingPermissionsException;
        }
    }

    private static async Task<Diagnostics> React(CommandContext ctx, ulong argMessageId, string argReactionId)
    {
        try
        {
            var message = await ctx.Channel.GetMessageAsync(argMessageId);
            if (message == null)
                return Diagnostics.NPTInvalidMessageException;

            DiscordEmoji emoji;
            if (argReactionId.StartsWith("<:")) //custom
            {
                //eg: <:cs:1262197231747072122>
                var emojiId = argReactionId.Split(':')[2].Trim('>');
                emoji = DiscordEmoji.FromGuildEmote(ctx.Client, ulong.Parse(emojiId));
            }
            else //default reaction
            {
                emoji = DiscordEmoji.FromName(ctx.Client, argReactionId);
            }

            //adds
            await message.CreateReactionAsync(emoji);
            return Diagnostics.Success;
        }
        catch (Exception)
        {
            return Diagnostics.DeniedException;
        }
    }

    private static async Task<Diagnostics> Unban(CommandContext ctx, ulong userId, string reason)
    {
        try{
            var user = await ctx.Guild.GetMemberAsync(userId);
            if (user == null)
                return Diagnostics.NPTInvalidUserException;

            await ctx.Guild.UnbanMemberAsync(userId, reason);
            return Diagnostics.Success;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return Diagnostics.NPTMissingPermissionsException;
        }
    }
}
