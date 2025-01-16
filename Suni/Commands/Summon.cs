using System.Collections.Generic;
using Suni.Suni.Configuration.Interfaces;

namespace Sun.Commands;

[Command("summon")]
[InteractionInstallType(DiscordApplicationIntegrationType.GuildInstall)]
[InteractionAllowedContexts(DiscordInteractionContextType.Guild)]
public class Summon(IAppConfig config)
{
    [Command("preceptor")]
    [InteractionInstallType(DiscordApplicationIntegrationType.GuildInstall)]
    [InteractionAllowedContexts(DiscordInteractionContextType.Guild)]
    public async Task SummonPreceptor(CommandContext ctx,
        [Parameter("people")] string people)
    {
        try
        {
            //getting the webhook
            var webhooks = await ctx.Channel.GetWebhooksAsync();
            var webhook = webhooks.FirstOrDefault(w => w.Name == "Preceptor");

            //not found: create a new one
            if (webhook == null)
                webhook = await ctx.Channel.CreateWebhookAsync("Preceptor", await Sun.ImageModels.Basics.ImagemPreceptor());

            //using a existing webhook

            await webhook.ExecuteAsync(new DiscordWebhookBuilder()
                .AddMentions(new List<IMention> { UserMention.All })
                .WithContent($"{ctx.User.Mention} " + await Functions.Functions.GetPeople(config, "preceptor")));
        }
        catch (Exception)
        {
            await ctx.RespondAsync("Erro ao criar/usar a webhook.");
        }
    }

    [Command("light")]
    [InteractionInstallType(DiscordApplicationIntegrationType.GuildInstall)]
    [InteractionAllowedContexts(DiscordInteractionContextType.Guild)]
    public async Task SummonLight(CommandContext ctx,
        [Parameter("people")] string people)
    {
        try
        {
            var webhooks = await ctx.Channel.GetWebhooksAsync();
            var webhook = webhooks.FirstOrDefault(w => w.Name == "Light");

            if (webhook == null)
                webhook = await ctx.Channel.CreateWebhookAsync("Lightzitos", await Sun.ImageModels.Basics.ImagemLightzinho());

            await webhook.ExecuteAsync(new DiscordWebhookBuilder()
                .AddMentions(new List<IMention> { UserMention.All })
                .WithContent($"{ctx.User.Mention} {await Functions.Functions.GetPeople(config, "lightzinho")}"));
        }
        catch (Exception)
        {
            await ctx.RespondAsync("Erro ao criar/usar a webhook.");
        }
    }
}