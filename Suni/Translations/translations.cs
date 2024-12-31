using System.Collections.Generic;

namespace Sun.Globalization
{
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
            private readonly Dictionary<string, string> _baseMessages;

            public GroupTranslationsMessages(string language)
            {
                _baseMessages = GetPortugueseMessages(); //This is Suni's base language, where you will always receive translations first.

                _messages = language switch
                {
                    "PT" => _baseMessages,
                    "RU" => GetRussianMessages(),
                    _ => GetEnglishMessages()
                };
            }
            public double GetTranslationCoveragePercentage()
            {
                int totalKeys = _baseMessages.Count;
                int matchingKeys = _messages.Count(pair => 
                    _baseMessages.ContainsKey(pair.Key) );//&& !string.IsNullOrEmpty(pair.Value)
                
                return (double)matchingKeys / totalKeys * 100;
            }

            //group of messages for a single command

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


            //base translations, located in this file
            //pt
            internal Dictionary<string, string> GetPortugueseMessages()
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
        }
    }
}
