using DSharpPlus.Commands.EventArgs;

namespace Suni.Suni.Events
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