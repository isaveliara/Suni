//usage:
//
//npt::unban(Sorry!) -> <user id>
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
        public static async Task<Diagnostics> Unban(CommandContext ctx, ulong userId, string reason)
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
