using System.Collections.Generic;
using System.Text.RegularExpressions;
using DSharpPlus.EventArgs;
using Sun.Functions.Romance;

namespace Sun.Events
{
    public partial class ReactionEvents
    {
        internal static async Task On_addedReaction(DiscordClient client, MessageReactionAddedEventArgs e)
        {
            if (e.Emoji.GetDiscordName() != ":heart:")
            {
                Console.WriteLine($"não é o emoji. ({e.Emoji.GetDiscordName()})");
                return;
            }

            if (e.User.IsBot)
            {
                Console.WriteLine("É bot.");
                return;
            }
            
            if (e.Message.Author == null || !e.Message.Author.IsCurrent)
            {
                Console.WriteLine($"Não é mensagem do próprio bot ou então uma mensagem antiga.");
                return;
            }
            
            bool botReacted = false;

            await foreach (var user in e.Message.GetReactionsAsync(e.Emoji))
            {
                if (user.Id == client.CurrentUser.Id)
                {
                    botReacted = true;
                    break;
                }
            }
            if (!botReacted)
            {
                Console.WriteLine("o bot não reagiu a mensagem dele para ser um casamento valido");
                return;
            }
            
            //get the users
            var matchU1 = Regex.Match(e.Message.Content, @"<@!?(\d+)>");
            var matchU2 = Regex.Match(e.Message.Embeds.First().Description[..32], @"<@!?(\d+)>");
            if (!matchU1.Success || !matchU2.Success)
            {
                Console.WriteLine($"Não foi possível encontrar os usuários.\n    content <{e.Message.Content}>\n    embed content <{e.Message.Embeds.First().Description[..32]}>");
                return;
            }

            var u1 = matchU1.Groups[1].Value;
            var u2 = matchU2.Groups[1].Value;
            //parse values to ulong
            var u1Id = ulong.Parse(u1);
            var u2Id = ulong.Parse(u2);

            if (e.User.Id.ToString() != u1)
            {
                Console.WriteLine($"Invalido: A proposta lançada para {u1} foi reagida por ({e.User.Id})");
                return;
            }
            
            Console.WriteLine($"<{u1}> aceitou o(a) <{u2}>");
            var language = new Functions.DB.DBMethods().GetUserLanguage(e.User.Id, lang: null, userName:e.User.Username, avatar:e.User.AvatarUrl);
            var tr = new Globalization.Using(language);
            (string _, string _, string _, string _, string noMoney, string success) = tr.Commands.GetMarryMessages(0, $"<@{u1}>", $"<@{u2}>");
            bool re = RomanceMethods.MarryUsers(u1Id, u2Id, true);
            if (!re)
            {
                await e.Message.RespondAsync(noMoney);
                return;
            }

            await e.Message.RespondAsync(new DiscordMessageBuilder()
                .WithAllowedMentions(new List<IMention> { UserMention.All })
                .WithContent(success));
        }
    }
}
