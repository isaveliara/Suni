using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;

namespace Sun.Events
{
    public partial class MessageEvents
    {
        internal static async Task On_message(DiscordClient client, MessageCreatedEventArgs args)
        {
            Console.WriteLine($"new message!");
            await Task.CompletedTask;
        }
    }
}