using Suni.Suni.NikoSharp.Core;
using Suni.Suni.NikoSharp.Core.Evaluator;
using Suni.Suni.NikoSharp.Data;
using Suni.Suni.NikoSharp.Formalizer;
namespace Suni;

public class Tests
{
    internal static async Task RunNptTester()
    {
        Console.Clear();
        Console.WriteLine($"Running SuniNPT '{SunClassBot.SuniV}'.\nType '#help' for help.\n\n");
        bool isEval = false;
        CommandContext ctx = null;
        
        while (true)
        {
            Console.ResetColor();

            if (isEval)
                Console.ForegroundColor = ConsoleColor.Cyan;
            
            Console.Write("> ");
            string code = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(code))
                break;

            if (code == "#help")
            {
                Console.WriteLine(@"Commands:
                #help: shows this.
                #eval: toggle to evaluate mode.
                #close: kit the program.
                
                | write your code in one line to run it. to kit, just send nothing.");
                continue;
            }
            else if (code == "#eval")
            {
                Console.ResetColor();
                isEval = !isEval;
                continue;
            }
            else if (code == "#close")
                break;

            Console.Clear();

            if (isEval)
            {
                FormalizingScript formalizingScript = new FormalizingScript(code, ctx);
                EnvironmentDataContext data = formalizingScript.GetFormalized;
                string formalizedExpression = data.Lines[0];

                var (resultEval, diagnostic, resultMessage) = NptEvaluator.EvaluateExpression(formalizedExpression, data);
                Console.WriteLine($"Result of Evaluation for '{formalizedExpression}' :");

                if (diagnostic != Diagnostics.Success)
                    Console.ForegroundColor = ConsoleColor.Red;
                else
                    Console.ForegroundColor = ConsoleColor.Yellow;
                
                Console.WriteLine($"{resultEval}\nWhith Result: {diagnostic}:\n" + resultMessage?? "");
                continue;
            }

            //building response
            string response = $"Result (Debugging) of SuniNPT code '{SunClassBot.SuniV}' is here:\n----------------------------------------";
            NptSystem parser = new NptSystem(code, ctx);
            Console.ForegroundColor = ConsoleColor.Gray; //changes the debug color
            var result = await parser.ParseScriptAsync();
            //here the console color can be reseted.

            if (result.result == Diagnostics.Forgotten)
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("script droped.");
                continue;
            }

            //first output
            foreach (var output in result.outputs)
                response += $"\n{output}";
            response += "\n\nDEBUG:\n";

            //debug
            foreach (var debug in result.debugs)
                response += $"\n    {debug}";

            Console.WriteLine($"\n\n{response}\n----------------------------------------");
            //aditional result
            if (result.result == Diagnostics.Success)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Result Program: **{result.result}**\n[Finished]");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Result Program: **{result.result}**\n[Finished]");
            }
        }

        //normal program running
        Console.ForegroundColor = ConsoleColor.DarkMagenta;
        Console.WriteLine("Running Suni;");
        Console.ResetColor();
    }
}