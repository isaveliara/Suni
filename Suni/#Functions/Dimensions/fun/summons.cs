using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Sun.Dimensions.Fun
{
    public partial class FunPre : BaseCommandModule
    {
        [Group("summon")] [Aliases("invocar","spawnar")] [GuildOnly]
        public class SummonPrefixCommandsGroup : BaseCommandModule
        {
            [Command("preceptor")]
            public async Task PREFIXCommandSummonPreceptor(CommandContext ctx)
            {
                try{
                    //getting the webhook
                    var webhooks = await ctx.Channel.GetWebhooksAsync();
                    var webhook = webhooks.FirstOrDefault(w => w.Name == "Preceptor");

                    //not found: create a new one
                    if (webhook == null)
                        webhook = await ctx.Channel.CreateWebhookAsync("Preceptor", await Sun.ImageModels.Basics.ImagemPreceptor());
                    
                    //using a existing webhook
                    await webhook.ExecuteAsync(new DiscordWebhookBuilder()
                        .AddMentions(new List<IMention> { UserMention.All })
                        .WithContent($"{ctx.User.Mention} " + await Functions.Functions.GetPeople("preceptor")));
                }
                catch (Exception)
                {
                    await ctx.RespondAsync("Erro ao criar/usar a webhook.");
                }
            }

            //join the commands into a single, this is more logic, but I'm lazy now :3
            [Command("light")]
            public async Task PREFIXCommandSummonLight(CommandContext ctx)
            {
                try{
                    var webhooks = await ctx.Channel.GetWebhooksAsync();
                    var webhook = webhooks.FirstOrDefault(w => w.Name == "Light");

                    if (webhook == null)
                        webhook = await ctx.Channel.CreateWebhookAsync("Lightzitos", await Sun.ImageModels.Basics.ImagemLightzinho());
                    
                    await webhook.ExecuteAsync(new DiscordWebhookBuilder()
                        .AddMentions(new List<IMention> { UserMention.All })
                        .WithContent($"{ctx.User.Mention} {await Functions.Functions.GetPeople("lightzinho")}"));
                }
                catch (Exception){
                    await ctx.RespondAsync("Erro ao criar/usar a webhook.");
                }
            }
        }
    }
}