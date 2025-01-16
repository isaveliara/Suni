using System;
using System.Collections.Generic;
using DSharpPlus.Commands;

namespace Suni.Suni
{
    public class Helpers
    {
        public static void RegisterAllCommands(CommandsExtension e)
        {
            //owner server
            List<Type> privateInteractionCommands = new List<Type>
            {

            };
            //user install and guild install commands
            List<Type> publicInteractionCommands = new List<Type>
            {
                typeof(Sun.Commands.AboutMe),
                typeof(Sun.Commands.Calculate),
                typeof(Sun.Commands.ContextMenus.Found_Commands),
                typeof(Sun.Commands.Dice),
                typeof(Sun.Commands.NptCommands),
                typeof(Sun.Commands.Romance),
                typeof(Sun.Commands.RegexCommandsGroup),
                typeof(Sun.Commands.AndresCommandsGroup),
                typeof(Sun.Commands.LanguageCommands),
                typeof(Sun.Commands.CustomNptCommands),
            };
            //guild install commands
            List<Type> ProtectedInteractionCommands = new List<Type>
            {
                typeof(Sun.Commands.Quiz),
                typeof(Sun.Commands.Summon),
                typeof(Sun.Commands.TestCommands),
            };

            //e.AddCommands(privateInteractionCommands, 123456789);
            e.AddCommands(publicInteractionCommands);
            e.AddCommands(ProtectedInteractionCommands);


        }
    }
}