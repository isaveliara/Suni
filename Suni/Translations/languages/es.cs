using System.Collections.Generic;

namespace Sun.Globalization;

public partial class Using
{
    public partial class GroupTranslationsMessages
    {
        //es
        internal Dictionary<string, string> GetSpanishMessages()
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