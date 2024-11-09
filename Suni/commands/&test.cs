using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Sun.Functions;
using System;

namespace Sun.PrefixCommands
{
    public partial class Miscellaneous : BaseCommandModule
    {
        [Command("test")] [RequireOwner]
        public async Task PREFIXCommandTest(CommandContext ctx)
        {
            var db = new Sun.Functions.DB.Methods();
            foreach (var m in ctx.Guild.Members.Values)
            {
                DiscordUser u = await ctx.Client.GetUserAsync(m.Id);
                string lang = u.Locale;
                Console.WriteLine($"Usuário {m.Username} tem o idioma {lang} detectado.");
                    
                db.InsertUser(userId: m.Id, username: m.Username, avatarUrl: m.AvatarUrl,
                              marriedWith: null, balance: 0, flags: "", badges: "user",
                              eventData: "", primaryLang: Sun.Functions.DB.LanguageStatusTypes.FROM_CLIENT,
                              status: Sun.Functions.DB.UserStatusTypes.client, xp: 0, reputation: 0,
                              commandNu: 0, lastActive: DateTime.Now, DBInsertOrIgnore:true);
                await Task.CompletedTask;
            }
        }
    }
}