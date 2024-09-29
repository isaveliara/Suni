using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using System;
using System.Collections.Generic;

namespace ScriptInterpreter
{
    public static partial class NptEntitie
    {
        public static async Task<Diagnostics> Controler(string methodName, List<string> args, string pointer, CommandContext ctx)
        {
            //set a try-catch here for controller of args and detailed exception inf
            Diagnostics result;
            try{
                switch (methodName)
                {
                    case "log": //npt::log(My Message) -> 1234567891011121314
                        ulong argChannel = ulong.Parse(pointer);
                        string argMessage = string.Join('\0',args);//why?
                        result = await NptEntitie.Log(ctx, argChannel, argMessage);
                        break;
                    case "react": //npt::react(:x:) -> <message id>
                    
                    default:
                        result = Diagnostics.NotFoundObjectException;
                        break;
                }
            }
            catch (Exception)
            {
                result = Diagnostics.UnknowException;
            }
            return result;
        }
    }
}

//test

//string script = @"
//    --definitions--
//    @set<ban_duration, '27days'>
//    --end--
//
//    npt::BanAsync(@get<ban_duration>, 'Fez alguma coisa') -> 12345678910
//    sys::Object('arg1', 'arg2', 99) -> Pointer
//    ";