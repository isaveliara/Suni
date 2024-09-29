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
                        ulong argChannelId = ulong.Parse(pointer);
                        string argContentMessage = string.Join('\0',args);//why?
                        result = await NptEntitie.Log(ctx, argChannelId, argContentMessage);
                        break;
                    case "react": //npt::react(:x:) -> <message id>
                        ulong argMessageId = ulong.Parse(pointer);
                        string argReactionId = args[0];
                        result = await NptEntitie.React(ctx, argMessageId, argReactionId);
                        break;
                    case "ban": //npt::ban(You broke a rule!) -> <user id>
                        ulong userId = ulong.Parse(pointer);
                        result = await NptEntitie.Ban(ctx, userId, string.Join('\0',args));//why
                        break;
                    case "unban": //npt::unban(Sorry!) -> <user id>
                        result = await NptEntitie.Ban(ctx, ulong.Parse(pointer), string.Join('\0',args));//why
                        break;
                    default: //npt::invalidmethod() -> null
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