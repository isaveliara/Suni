using System;
using System.Collections.Generic;
using DSharpPlus.CommandsNext;
using DSharpPlus.SlashCommands;
using Sun.Globalization;

namespace Sun.Functions.DB
{
    public partial class DBMethods
    {
        public static SuniSupportedLanguages tryFoundUserLang(ulong userId, string lang = null, string userName = null, string avatar = null)
        {
            var db = new DBMethods();
            var values = db.GetUserFields(userId: userId, new List<string> { "primary_lang" });
            string foundDBUserLang = null;
            if (values.TryGetValue("primary_lang", out var value) && value != null)
                foundDBUserLang = value.ToString();
            
            SuniSupportedLanguages supported;

            //unknow user
            if (foundDBUserLang == null)
            {
                Console.WriteLine("Unknow user:");
                //can't set lang
                if (lang == null)
                {
                    //store user with FROM_CLIENT value
                    db.InsertUser(userId, userName, avatar, commandNu:1, primaryLang: SuniSupportedLanguages.FROM_CLIENT);
                    Console.WriteLine($"    User {userId} has stored with FROM_CLIENT value");
                    return SuniSupportedLanguages.PT;
                }

                //can set lang
                Console.WriteLine("    can set lang");
                supported = GlobalizationMethods.TryConvertLanguageToSupported(lang);
                db.InsertUser(userId, userName, avatar, commandNu:1, primaryLang: supported);
                return supported;
            }

            //knowed user, so we try to get the lang from the DB
            
            //if lang exists and foundDBUserLang is a valid lang:
            if (foundDBUserLang == "FROM_CLIENT" && lang != null){
                supported = GlobalizationMethods.TryConvertLanguageToSupported(lang);

                db.UpdateUser(userId, new Dictionary<string, object>
                {
                    { "primary_lang", supported.ToString() }
                });
                Console.WriteLine($"Set FROM_CLIENT language value to {supported} for the {userId} user");
                return supported;
            }

            if (foundDBUserLang != "FROM_CLIENT")
            {
                supported = GlobalizationMethods.TryConvertLanguageToSupported(foundDBUserLang);
                return supported;
            }

            //in this case, user exists in DB, has FROM_CLIENT value, and can't get the Locale lang, returning PT as default
            Console.WriteLine("returning PT as default because Locale is null and user doesnt eixsts on DB (or has FROM_CLIENT as primary_lang)");
            return SuniSupportedLanguages.PT;
            //Console.WriteLine($"Found lang in DB: {foundDBUserLang} >Converted: {supported} - Found in Locale: {lang}");
        }
    }
}
