namespace Sun.Commands;

public partial class TestCommands
{
    [Command("test")]
    [InteractionInstallType(DiscordApplicationIntegrationType.GuildInstall)]
    [InteractionAllowedContexts(DiscordInteractionContextType.Guild)]
    public async Task TestCommandInsertUser(CommandContext ctx)
    {
        var db = new Sun.Functions.DB.DBMethods();
        foreach (var m in ctx.Guild.Members.Values)
        {
            DiscordUser u = await ctx.Client.GetUserAsync(m.Id);
            string lang = u.Locale;
            Console.WriteLine($"Usuário {m.Username} tem o idioma {lang} detectado.");
                
            db.InsertUser(userId: m.Id, username: m.Username, avatarUrl: m.AvatarUrl,
                            marriedWith: null, balance: 15000, flags: "", badges: "user",
                            eventData: "", primaryLang: Sun.Globalization.SuniSupportedLanguages.FROM_CLIENT,
                            status: Sun.Functions.DB.UserStatusTypes.client, xp: 0, reputation: 0,
                            commandNu: 0, lastActive: DateTime.Now, DBInsertOrIgnore:true);
        }
        await ctx.RespondAsync("OK.");
    }
}