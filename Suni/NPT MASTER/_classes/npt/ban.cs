//usage:
//
//npt::ban(You broke a rule!) -> <user id>
//
//

using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System;

namespace ScriptInterpreter
{
    //npt entities
    public static partial class NptEntitie
    {
        public static async Task<Diagnostics> Ban(CommandContext ctx, ulong userId, string reason)
        {
            try{
                var user = await ctx.Guild.GetMemberAsync(userId);
                if (user == null)
                    return Diagnostics.NPTInvalidUserException;

                await ctx.Guild.BanMemberAsync(user, reason:reason);
                return Diagnostics.Success;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Diagnostics.NPTMissingPermissionsException;
            }
        }
    }
}
