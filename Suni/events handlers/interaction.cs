using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace Sun.HandlerFunctions
{
    public class Components
    {
        internal static async Task InteractionEventHandler(DiscordClient sender, ComponentInteractionCreateEventArgs e)
        {
            //dropdown--
            if (e.Id == "dropDownList" && e.Interaction.Data.ComponentType == ComponentType.StringSelect)
            {
                Console.WriteLine("stringselect");
                var options = e.Values;
                foreach (var option in options)
                {
                    switch (option)
                    {
                        case "option1":
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.Pong);
                            return;
                        default:
                            await e.Interaction.DeferAsync();
                            return;
                    }
                }
                return;
            }

            //buttons--
            Console.WriteLine(".");
            switch (e.Interaction.Data.CustomId)
            {
                case "send_this":
                    var originalEmbed = e.Message.Embeds.First();
                    var originalContent = e.Message.Content;

                    var copiedMessage = new DiscordInteractionResponseBuilder()
                        .WithContent(originalContent)
                        .AddEmbed(originalEmbed);
                        
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, copiedMessage);
                    return;
                default:
                    await e.Interaction.DeferAsync();
                    return;
            }
        }

        internal static async Task ModalsHandler(DiscordClient sender, ModalSubmitEventArgs e)
        {
            if (e.Interaction.Type == InteractionType.ModalSubmit)
            {
                var values = e.Values;
                await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent($"{e.Interaction.User.Username} submited {values.Values.First()}"));
            }
        }
    }
}