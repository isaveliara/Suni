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
                typeof(Suni.Commands.AboutMe),
                typeof(Suni.Commands.Calculate),
                typeof(Suni.Commands.Menus.FoundCommands),
                typeof(Suni.Commands.Dice),
                typeof(Suni.Commands.NptCommands),
                typeof(Suni.Commands.Romance),
                typeof(Suni.Commands.UsesAutoComplete.RegexCommandsGroup),
                typeof(Suni.Commands.UsesAutoComplete.AndresCommandsGroup),
                typeof(Suni.Commands.LanguageCommands),
                typeof(Suni.Commands.CustomNptCommands),
            };
            //guild install commands
            List<Type> ProtectedInteractionCommands = new List<Type>
            {
                typeof(Suni.Commands.Quiz),
                typeof(Suni.Commands.Summon),
                typeof(Suni.Commands.TestCommands),
            };

            //e.AddCommands(privateInteractionCommands, 123456789);
            e.AddCommands(publicInteractionCommands);
            e.AddCommands(ProtectedInteractionCommands);


        }
    }
}