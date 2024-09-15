/*
File responsible for generate the messages/other things to be used by suni.
The translation sys. could be here, but I got lazy and decided to wait until square cloud BD support is ready.
*/

using System;

namespace SunFunctions
{
    public partial class Functions
    {
        public static string HelloWorld()
            =>
                "Hello World!";
        
        internal static string GetShipMessage(int percent, string u1, string u2)
            => percent switch
            {
                0 => "...",
                <= 13 => "Esqueça :headskull:",
                <= 24 => "Não existe motivo para que esse casal exista!",
                <= 49 => "Improvável! Vamos torcer por esses dois...",
                <= 69 => "Por que não dar certo? :heart:",
                <= 89 => new Random().Next(1,3) == 1
                         ? $"O coração de {u1} aquece por {u2}"
                         : $"O coração de {u2} aquece por {u1}",
                   90 => "Ownn... Esse seria o casal mais fofinho que eu já vi",
                   _ => "?"
            };
        //others
    }
}

