using System.Collections.Generic;

namespace Sun.Globalization;

public partial class Using
{
    public partial class GroupTranslationsMessages
    {
        //ru
        internal Dictionary<string, string> GetRussianMessages()
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
    }
}