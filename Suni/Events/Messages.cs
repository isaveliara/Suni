using DSharpPlus.EventArgs;

namespace Sun.Events
{
    public partial class MessageEvents
    {
        internal static async Task On_message(DiscordClient client, MessageCreatedEventArgs e)
        {
            Console.WriteLine($"new message!");

            if (e.Author.IsBot)
                return;
            
            //seed for decide actions 
            int messageSeed = new Random().Next(1, 100000);

            if (messageSeed % 333 == 0)
                await e.Message.RespondAsync(e.Message.Content);

            if (messageSeed % 100 == 0)
            {
                var emojis = new[]
                {
                    DiscordEmoji.FromName(client, ":thumbsup:"),
                    DiscordEmoji.FromName(client, ":smile:"),
                    DiscordEmoji.FromName(client, ":heart:"),
                    DiscordEmoji.FromName(client, ":fire:"),
                    DiscordEmoji.FromName(client, ":ox:"),
                    DiscordEmoji.FromName(client, ":star:"),
                    DiscordEmoji.FromName(client, ":sweat:"),
                };

                int emojiIndex = messageSeed % emojis.Length;
                var selectedEmoji = emojis[emojiIndex];

                await e.Message.CreateReactionAsync(selectedEmoji);
            }
        }
    }
}
