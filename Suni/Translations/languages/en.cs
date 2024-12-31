using System.Collections.Generic;

namespace Sun.Globalization;

public partial class Using
{
    public partial class GroupTranslationsMessages
    {
        //en
        internal Dictionary<string, string> GetEnglishMessages()
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
    }
}