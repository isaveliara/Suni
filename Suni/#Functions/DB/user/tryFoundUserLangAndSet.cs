using System;
using System.Collections.Generic;
using DSharpPlus.SlashCommands;
using Sun.Globalization;

namespace Sun.Functions.DB
{
    public partial class DBMethods
    {
        public static SuniSupportedLanguages tryFoundUserLang(ulong userId, string lang = null)
        {
            var db = new Methods();
            var values = db.GetUserFields(userId: userId, new List<string> { "primary_lang" });
            string foundDBUserLang = null;

            Globalization.SuniSupportedLanguages supported;
            if (values.TryGetValue("primary_lang", out var value) && value != null)
            {
                foundDBUserLang = value.ToString();
            }

            if (foundDBUserLang == "FROM_CLIENT" && lang != null){
                //means that the user has not set a language yet and a language has a provided to set
                supported = GlobalizationMethods.TryConvertLanguageToSupported(lang);

                db.UpdateUser(userId, new Dictionary<string, object>
                {
                    { "primary_lang", supported.ToString() }
                });
                return supported;
            }

            supported = GlobalizationMethods.TryConvertLanguageToSupported(foundDBUserLang.ToString());

            Console.WriteLine($"Found lang in DB: {foundDBUserLang} >Converted: {supported} - Found in Locale: {lang}");
            return GlobalizationMethods.TryConvertLanguageToSupported(supported.ToString());
        }
    }
}
