using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;
using RestSharp;

namespace HandlerFunctions.Listeners
{
    public partial class Handler
    {
        internal static async Task FatherMemberUpdated(DiscordClient sender, GuildMemberUpdatedEventArgs  e)
        {
            if (e.RolesBefore.Count < e.RolesAfter.Count)
            {
                //receive
                await HandlerFunctions.Listeners.RECEIVE.Role(e);
                /*
                var newRole = e.RolesAfter.Except(e.RolesBefore);
                foreach (var role in newRole)
                {
                    await e.Member.SendMessageAsync($"Você recebeu o cargo: {role.Name}");
                }
                */
            }
            else if (e.RolesBefore.Count > e.RolesAfter.Count)
            {
                //take
                /*
                var removedRole = e.RolesBefore.Except(e.RolesAfter);
                foreach (var role in removedRole)
                {
                    await e.Member.SendMessageAsync($"O cargo {role.Name} foi removido de você.");
                }
                */
            }
        }
    }
}