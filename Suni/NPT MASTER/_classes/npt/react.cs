//usage:
//
//npt::react(:x:) -> <message id>
//
//

using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System;

namespace Sun.NPT.ScriptInterpreter
{
    //npt entities
    public static partial class NptEntitie
    {
        //react on a message
        public static async Task<Diagnostics> React(CommandContext ctx, ulong argMessageId, string argReactionId)
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
    }
}
