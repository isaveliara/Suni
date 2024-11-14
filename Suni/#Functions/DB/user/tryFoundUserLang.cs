using System;
using System.Collections.Generic;
using DSharpPlus.SlashCommands;
using Sun.Globalization;

namespace Sun.Functions.DB
{
    public partial class DBMethods
    {
        //for prefix commands, where FROM_LOCALE is unreachable
        public static SuniSupportedLanguages tryFoundUserLang(ulong userId, string lang = null)
        {
            if (lang != null)
            {
                Console.WriteLine($"Lang not null: {lang}");
                return GlobalizationMethods.TryConvertLanguageToSupported(lang);
            }
            
            var db = new Sun.Functions.DB.Methods();
            var values = db.GetUserFields(userId: userId, new List<string> { "primary_lang" });
            string foundLang = "PT"; //default

            if (values.TryGetValue("primary_lang", out var value) && value != null)
                foundLang = value.ToString();
            
            Console.WriteLine($"Found lang in DB: {foundLang}");
            return GlobalizationMethods.TryConvertLanguageToSupported(foundLang);
        }
    }
}
