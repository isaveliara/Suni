using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace Sun.Dimensions.Romance
{
    //prefix command

    public partial class Pre : BaseCommandModule
    {
        [Command("marry")]
        public async Task PREFIXCommandMarry(CommandContext ctx,
        [Option("user","user to marry")] DiscordUser user)
        {
            //isnt bot:
            if (user.IsBot)
            {
                await ctx.RespondAsync($"{ctx.User.Username} bobinho, você não pode se casar com bots!");
                return;
            }

            //isnt same user:
            if (user.Id == ctx.User.Id)
            {
                await ctx.RespondAsync($"{ctx.User.Username} bobinho, você não pode se casar contigo mesmo!");
                return;
            }
            
            //creating
            DiscordEmbed embed = new DiscordEmbedBuilder()
                .WithTitle("O amor está no ar...")
                .WithDescription($"{ctx.User.Mention} te enviou uma proprosta de casamento!" +
                        "\nReaja com :heart: para aceitar.." +
                        "\nLembre-se: casar custará **200 moedas** (metade pra cada usuário) **todos** os dias, e mais **20k de moedas** (metade pra cada usuário também) como **inicialização**! "
                        );

            var msg = await ctx.RespondAsync(new DiscordMessageBuilder()
                .WithAllowedMentions(new List<IMention> { UserMention.All })
                .WithContent($"{user.Mention} parece que você recebeu uma proprosta...")
                .AddEmbed(embed)
                );
            
            await msg.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":heart:"));

            //wait for you acceptance.     
            
            //ignoring that u didn't accept it yet, i know whats best for u.

            //executing:
            bool re = Romance.Methods.MarryAUsers(ctx.User.Id, user.Id, true);
            if (!re)
            {
                await ctx.RespondAsync($":x: | {ctx.User.Mention} {user.Mention} É necessário 20.000 para poder formar um casal, e em seus fundos não alcançam esse dinheiro!");
                return;
            }

            await ctx.RespondAsync(new DiscordMessageBuilder()
                .WithAllowedMentions(new List<IMention> { UserMention.All })
                .WithContent($"{ctx.User.Mention}, {user.Mention}, vocês estão casados agora! Felicidades para vocês dois.."));
        }
    }
}