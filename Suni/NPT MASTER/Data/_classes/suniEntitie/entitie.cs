//controler of suni class

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;

namespace Sun.NPT.ScriptInterpreter
{
    public static partial class SuniEntitie
    {
        public static async Task<Diagnostics> Controler(string methodName, List<string> args, string pointer, CommandContext ctx)
        {
            await Task.CompletedTask;
            
            //set a try-catch here for controller of args
            Diagnostics result;
            try{
                switch (methodName)
                {
                    case "define_me": //suni::define_me(cmd query) -> db
                        Console.WriteLine("Err");
                        result = Diagnostics.Success;
                        break;
                    default: //suni::invalidmethod() -> null
                        result = Diagnostics.NotFoundObjectException;
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                result = Diagnostics.UnknowException;
            }
            return result;
        }

        //
    }
}