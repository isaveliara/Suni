using System;
using System.Collections.Generic;

namespace Sun.Globalization
{
    public enum SuniSupportedLanguages{
        PT, EN, RU, FROM_CLIENT
    }
    public partial class GlobalizationMethods
    {
        public static SuniSupportedLanguages TryConvertLanguageToSupported(string lang)
        {
            if (string.IsNullOrWhiteSpace(lang))
                return SuniSupportedLanguages.PT; //default
            
            return lang.ToLower() switch
            {
                "pt" or "pt-br" => SuniSupportedLanguages.PT,
                "en" or "en-us" or "en-gb" => SuniSupportedLanguages.EN,
                "ru" or "ru-ru" => SuniSupportedLanguages.RU,
                _ => SuniSupportedLanguages.PT //default
            };
        }
    }

    public partial class Using
    {
        public SuniSupportedLanguages Language { get; }
        public GroupTranslationsMessages Commands { get; }
        public Using(SuniSupportedLanguages language){
            Language = language;
            Commands = new GroupTranslationsMessages(language.ToString());
        }

        public partial class GroupTranslationsMessages
        {
            private readonly Dictionary<string, string> _messages;
            public GroupTranslationsMessages(string language)
            {
                _messages = language switch
                {
                    "PT" => GetPortugueseMessages(),
                    "EN" => GetEnglishMessages(),
                    "RU" => GetRussianMessages(),
                    _ => GetRussianMessages() //default
                };
            }

            //Group of messages for a command

            public (string, string) GetUserinfoMessages()
            {
                return (null, null);
            }

            public (string, string) GetShipMessages(int percent, string u1, string u2)
            {
                string name = string.Format(_messages["Ship_Response"], u1.Substring(0,u1.Length/2)+u2.Substring(u2.Length/2));
                return (percent switch
                {
                    0 => "...", //same values
                    <= 13 => _messages["Ship_13"],
                    <= 24 => _messages["Ship_24"],
                    <= 49 => _messages["Ship_49"],
                    <= 69 => _messages["Ship_69"],
                    <= 89 => new Random().Next(1, 3) == 1
                             ? string.Format(_messages["Ship_89_1"], u1, u2)
                             : string.Format(_messages["Ship_89_2"], u2, u1),
                    90 => _messages["Ship_90"],
                    _ => "?"
                }, name); //message content response
            }


            //.===TRANSLATIONS===.

            //pt
            private Dictionary<string, string> GetPortugueseMessages()
                => new()
                {
                    //ship
                    { "Ship_Description", "Calcula a porcentagem de compatibilidade entre duas pessoas" },
                    { "Ship_Response",":heart: | O nome do casal seria {0}\n:heart: | Com uma probabilidade de {0}" },
                    { "Ship_13", "Esqueça :headskull:" },
                    { "Ship_24", "Não existe motivo para que esse casal exista!" },
                    { "Ship_49", "Improvável! Vamos torcer por esses dois..." },
                    { "Ship_69", "Por que não dar certo? :heart:" },
                    { "Ship_89_1", "O coração de {0} aquece por {1}" },
                    { "Ship_89_2", "O coração de {1} aquece por {0}" },
                    { "Ship_90", "Ownn... Esse seria o casal mais fofinho que eu já vi" },

                    //ping command
                    { "Ping_Description","Mostra a minha latência" },
                    { "Ping_Message", "Pong! :ping_pong:\n Latência: {0}ms" },

                    //values
                    { "Language", "Portuguese" }
                };
            
            //en
            private Dictionary<string, string> GetEnglishMessages()
                => new()
                {
                    //ship
                    { "Ship_Description", "Calculates the compatibility percentage between two people" },
                    { "Ship_Response", ":heart: | The couple's name would be {0}\n:heart: | With a probability of {0}" },
                    { "Ship_13", "Forget it :headskull:" },
                    { "Ship_24", "There's no reason for this couple to exist!" },
                    { "Ship_49", "Unlikely! Let's root for these two..." },
                    { "Ship_69", "Why wouldn't it work? :heart:" },
                    { "Ship_89_1", "{0}'s heart warms for {1}" },
                    { "Ship_89_2", "{1}'s heart warms for {0}" },
                    { "Ship_90", "Aww... This would be the cutest couple I've ever seen" },

                    //ping command
                    { "Ping_Description", "Shows my latency" },
                    { "Ping_Message", "Pong! :ping_pong:\n Latency: {0}ms" },

                    //values
                    { "Language", "English" }
                };
            
            //ru
            private Dictionary<string, string> GetRussianMessages()
                => new()
                {
                    //ship
                    { "Ship_Description", "Вычисляет процент совместимости между двумя людьми" },
                    { "Ship_Response", ":heart: | Имя пары было бы {0}\n:heart: | С вероятностью {0}" },
                    { "Ship_13", "Забудь oб этом :headskull:" },
                    { "Ship_24", "Нет причин, чтобы эта пара существовала!" },
                    { "Ship_49", "Маловероятно! Давайте пожелаем этим двоим удачи..." },
                    { "Ship_69", "Почему бы и нет? :heart:" },
                    { "Ship_89_1", "Сердце {0} согревается для {1}" },
                    { "Ship_89_2", "Сердце {1} согревается для {0}" },
                    { "Ship_90", "Ooo... Это была бы самая милая пара, которую я когда-либо видел" },

                    //ping command
                    { "Ping_Description", "Показывает мою задержку" },
                    { "Ping_Message", "Понг! :ping_pong:\n Задержка: {0}мс" },

                    //values
                    { "Language", "Russian" }
                };
            
            //es
            private Dictionary<string, string> GetSpanishMessages()
                => new()
                {
                    //ship
                    { "Ship_Description", "Calcula el porcentaje de compatibilidad entre dos personas" },
                    { "Ship_Response", ":heart: | El nombre de la pareja sería {0}\n:heart: | Con una probabilidad de {0}" },
                    { "Ship_13", "Olvídalo :headskull:" },
                    { "Ship_24", "¡No hay razón para que esta pareja exista!" },
                    { "Ship_49", "¡Improbable! Vamos a apoyar a estos dos..." },
                    { "Ship_69", "¿Por qué no funcionaría? :heart:" },
                    { "Ship_89_1", "El corazón de {0} se calienta por {1}" },
                    { "Ship_89_2", "El corazón de {1} se calienta por {0}" },
                    { "Ship_90", "Aww... Esta sería la pareja más linda que haya visto" },

                    //ping command
                    { "Ping_Description", "Muestra mi latencia" },
                    { "Ping_Message", "¡Pong! :ping_pong:\n Latencia: {0}ms" },

                    //values
                    { "Language", "Spanish" }
                };
        }
    }
}
