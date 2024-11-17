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

            public (string message_error, string embedTItle, string embedDescription, string content, string noMoney, string success) GetMarryMessages(byte error_id, string ctxUserMention, string userMention)
            {
                return (_messages[$"Marry_E{error_id}"], //0-3 errors
                        _messages["Marry_embedTitle"], //"O amor está no ar..."
                        string.Format(_messages["Marry_embedDescription"], ctxUserMention), //$"{0} te enviou uma proprosta de casamento!\nReaja com :heart: para aceitar..\nLembre-se: casar custará **200 moedas** (metade pra cada usuário) **todos** os dias, e mais **20k de moedas** (metade pra cada usuário também) como **inicialização**!"
                        string.Format(_messages["Marry_messageContent"], userMention), //$"{0} parece que você recebeu uma proprosta..."
                        string.Format(_messages["Marry_noMoney"], ctxUserMention, userMention), //$":x: | {0} {1} É necessário 20.000 para poder formar um casal, e em seus fundos não alcançam esse dinheiro!"
                        string.Format(_messages["Marry_messageContent_OnSuccess"], ctxUserMention, userMention) //$"{0}, {1}, vocês estão casados agora! Felicidades para os dois.."
                );
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

                    //marry
                    { "Marry_E0", null },
                    { "Marry_E1", "bobinho, não pode se casar com bots!" },
                    { "Marry_E2", "bobinho, não pode se casar contigo mesmo!" },
                    { "Marry_E3", "bobinho, não pode se casar com usuários já casados!" },
                    { "Marry_embedTitle", "O amor está no ar..." },
                    { "Marry_embedDescription", "{0} te enviou uma proprosta de casamento!\nReaja com :heart: para aceitar..\nLembre-se: casar custará **200 moedas** (metade pra cada usuário) **todos** os dias, e mais **20k de moedas** (metade pra cada usuário também) como **inicialização**!" },
                    { "Marry_messageContent", "{0} parece que você recebeu uma proprosta..." },
                    { "Marry_noMoney", ":x: | {0} {1} É necessário 20.000 para poder formar um casal, e em seus fundos não alcançam esse dinheiro!" },
                    { "Marry_messageContent_OnSuccess", "{0}, {1}, vocês estão casados agora! Felicidades para os dois.." },

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

                    //marry
                    { "Marry_E0", null },
                    { "Marry_E1", "silly, you can't marry bots!" },
                    { "Marry_E2", "silly, you can't marry yourself!" },
                    { "Marry_E3", "silly, you can't marry users who are already married!" },
                    { "Marry_embedTitle", "Love is in the air..." },
                    { "Marry_embedDescription", "{0} has sent you a marriage proposal!\nReact with :heart: to accept..\nRemember: marrying will cost **200 coins** (shared equally between both users) **daily**, and an initial fee of **20k coins** (also shared equally)!" },
                    { "Marry_messageContent", "{0}, it seems like you received a proposal..." },
                    { "Marry_noMoney", ":x: | {0} {1}, you need 20,000 coins to form a couple, but your funds are insufficient!" },
                    { "Marry_messageContent_OnSuccess", "{0}, {1}, you're now married! Best wishes to both of you!" },

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

                    //marry
                    { "Marry_E0", null },
                    { "Marry_E1", "глупышка, ты не можешь жениться на ботах!" },
                    { "Marry_E2", "глупышка, ты не можешь жениться на самом себе!" },
                    { "Marry_E3", "глупышка, ты не можешь жениться на уже женатых пользователях!" },
                    { "Marry_embedTitle", "Любовь витает в воздухе..." },
                    { "Marry_embedDescription", "{0} отправил(а) тебе предложение о браке!\nОтреагируй :heart:, чтобы согласиться..\nПомни: брак обойдется в **200 монет** (разделено поровну) **ежедневно**, и первоначальный взнос составит **20k монет** (также разделено поровну)!" },
                    { "Marry_messageContent", "{0}, похоже, ты получил(а) предложение..." },
                    { "Marry_noMoney", ":x: | {0} {1}, для создания пары требуется 20,000 монет, но у вас недостаточно средств!" },
                    { "Marry_messageContent_OnSuccess", "{0}, {1}, теперь вы женаты! Счастья вам обоим!" },

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

                    //marry
                    { "Marry_E0", null },
                    { "Marry_E1", "tontito(a), ¡no puedes casarte con bots!" },
                    { "Marry_E2", "tontito(a), ¡no puedes casarte contigo mismo(a)!" },
                    { "Marry_E3", "tontito(a), ¡no puedes casarte con usuarios que ya están casados!" },
                    { "Marry_embedTitle", "El amor está en el aire..." },
                    { "Marry_embedDescription", "¡{0} te ha enviado una propuesta de matrimonio!\nReacciona con :heart: para aceptar..\nRecuerda: casarse costará **200 monedas** (dividido entre ambos usuarios) **diariamente**, y una tarifa inicial de **20k monedas** (también dividida entre ambos)!" },
                    { "Marry_messageContent", "{0}, parece que has recibido una propuesta..." },
                    { "Marry_noMoney", ":x: | {0} {1}, necesitas 20.000 monedas para formar una pareja, ¡pero tus fondos no son suficientes!" },
                    { "Marry_messageContent_OnSuccess", "{0}, {1}, ¡ahora están casados! ¡Felicidades para ambos!" },

                    //ping command
                    { "Ping_Description", "Muestra mi latencia" },
                    { "Ping_Message", "¡Pong! :ping_pong:\n Latencia: {0}ms" },

                    //values
                    { "Language", "Spanish" }
                };
        }
    }
}
