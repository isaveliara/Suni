//usage:
//
//npt::log(My Message) -> 1234567891011121314
//
//

using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using System;

namespace ScriptInterpreter
{
    //npt entities
    public static partial class NptEntitie
    {
        //log in channel
        public static async Task<Diagnostics> Log(CommandContext ctx, ulong channelId, string message){
            try{
                if (!ctx.Guild.Channels.ContainsKey(channelId))
                    return Diagnostics.NPTInvalidChannelException;
                
                var channel = await ctx.Client.GetChannelAsync(channelId);
                await channel.SendMessageAsync(message); //sends
                return Diagnostics.Success;
            }
            catch (Exception){
                Console.WriteLine("waht");
                return Diagnostics.NPTMissingPermissionsException;
            }
        }
    }
}