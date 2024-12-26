//controler of npt class

using System.Threading.Tasks;
using DSharpPlus.Commands;
using System;
using System.Collections.Generic;
using DSharpPlus.Entities;

namespace Sun.NPT.ScriptInterpreter
{
    public static partial class NptEntitie
    {
        public static async Task<Diagnostics> Controler(string methodName, List<string> args, string pointer, CommandContext ctx)
        {
            //set a try-catch here for controller of args
            Diagnostics result;
            try{
                switch (methodName)
                {
                    case "log": //npt::log(My Message) -> 1234567891011121314
                        ulong argChannelId = ulong.Parse(pointer);
                        string argContentMessage = string.Join('\0',args);//why?
                        result = await NptEntitie.Log(ctx, argChannelId, argContentMessage);
                        break;
                    case "react": //npt::react(:x:) -> <message id>
                        ulong argMessageId = ulong.Parse(pointer);
                        string argReactionId = args[0];
                        result = await NptEntitie.React(ctx, argMessageId, argReactionId);
                        break;
                    case "ban": //npt::ban(You broke a rule!) -> <user id>
                        ulong userId = ulong.Parse(pointer);
                        result = await NptEntitie.Ban(ctx, userId, string.Join('\0',args));//why
                        break;
                    case "unban": //npt::unban(Sorry!) -> <user id>
                        result = await NptEntitie.Ban(ctx, ulong.Parse(pointer), string.Join('\0',args));//why
                        break;
                    default: //npt::invalidmethod() -> null
                        result = Diagnostics.NotFoundObjectException;
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                result = Diagnostics.UnknowException;
            }
            return result;
        }

        //methods of nptentitie
        public static readonly Dictionary<string, string> Methods = new Dictionary<string, string> {
            { "log", "npt" },
            { "react", "npt" },
            { "ban", "npt" },
            { "unban", "npt" },
        };

        
        //bans a member
        //
        //usage:
        //
        //npt::ban(You broke a rule!) -> <user id>
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

        //log in a channel
        //
        //usage:
        //
        //npt::log(My Message) -> 1234567891011121314
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


        //react on a message
        //
        //usage:
        //
        //npt::react(:x:) -> <message id>
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
                return Diagnostics.NPTDeniedException;
            }
        }

        //unban a member:
        //
        //usage:
        //
        //npt::unban(Sorry!) -> <user id>
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
}

//test

//string script = @"
//    --definitions--
//    @set<ban_duration, '27days'>
//    --end--
//
//    npt::BanAsync(@get<ban_duration>, 'Fez alguma coisa') -> 12345678910
//    sys::Object('arg1', 'arg2', 99) -> Pointer
//    ";