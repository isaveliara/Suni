using System;
using System.Threading.Tasks;
using DSharpPlus.Commands;
using DSharpPlus.Commands.EventArgs;

namespace Sun.Events
{
    public partial class ErrorEvents
    {
        internal static async Task CommandErrored(CommandsExtension sender, CommandErroredEventArgs args)
        {
            Console.WriteLine($"Err: {args.Exception}");
            await Task.CompletedTask;
        }
    }
}