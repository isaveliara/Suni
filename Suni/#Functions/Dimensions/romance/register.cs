using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;

namespace Sun.Dimensions.Romance
{
    //prefix command

    public partial class Pre : BaseCommandModule
    {
        [Command("marry")] [Cooldown(maxUses:1, resetAfter:10, CooldownBucketType.User)]
        public async Task PREFIXCommandMarry(CommandContext ctx,
        [Option("user","user to marry")] DiscordUser user)
        {
            byte error = 0;
            //string message_error = null;
            //isnt bot:
            if (user.IsBot)
                error = 1;

            //isnt same user:
            if (user.Id == ctx.User.Id)
                error = 2;

            //users are already married
            if (new Sun.Functions.DB.DBMethods().AreUsersMarried(ctx.User.Id, user.Id)){
                //message_error = $"";
                error = 3;
            }

            var language = Functions.DB.DBMethods.tryFoundUserLang(ctx.User.Id, lang: null, userName:ctx.User.Username, avatar:ctx.User.AvatarUrl);
            var tr = new Globalization.Using(language);
            (string message_error, string embedTItle, string embedDescription, string content, string noMoney, string success) = tr.Commands.GetMarryMessages(error, ctx.User.Mention, user.Mention);

            //some error:
            if (message_error != null)
            {
                await ctx.Channel.SendMessageAsync($"{ctx.User.Mention} {message_error} :x:");
                return;
            }
            
            //creating
            DiscordEmbed embed = new DiscordEmbedBuilder()
                .WithTitle(embedTItle)
                .WithDescription(embedDescription);

            var msg = await ctx.RespondAsync(new DiscordMessageBuilder()
                .WithAllowedMentions(new List<IMention> { UserMention.All })
                .WithContent(content)
                .AddEmbed(embed)
                );
            
            await msg.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":heart:"));

            //event
            bool re = RomanceMethods.MarryAUsers(ctx.User.Id, user.Id, true);
            if (!re)
            {
                await ctx.RespondAsync(noMoney);
                return;
            }

            await ctx.RespondAsync(new DiscordMessageBuilder()
                .WithAllowedMentions(new List<IMention> { UserMention.All })
                .WithContent(success));
        }
    }
}