//damn

using System.Collections.Generic;
using Sun.Globalization;

namespace Sun.Functions.DB
{
    public partial class DBMethods
    {
        public static bool SetUserLang(ulong userId, string newLanguage)
        {
            var supported = GlobalizationMethods.TryConvertLanguageToSupported(newLanguage);
            try
            {
                new DBMethods().UpdateUser(userId, new Dictionary<string, object>
                    {
                        { "primary_lang", supported.ToString() }
                    });
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Falha ao redefinir idioma do usuário '{userId}': {ex.Message}");
                return false;
            }
        }

        public SuniSupportedLanguages GetUserLanguage(ulong userId, string lang = null, string userName = null, string avatar = null)
        {
            var userFields = GetUserFields(userId, new List<string> { "primary_lang" });
            if (userFields == null)
                return HandleUnknownUser(userId, lang, userName, avatar);
            if (!userFields.TryGetValue("primary_lang", out var value) || value == null)
                return HandleUnknownUser(userId, lang, userName, avatar);

            var foundLang = value.ToString();
            if (foundLang == "FROM_CLIENT" && lang != null)
            {
                var supportedLang = GlobalizationMethods.TryConvertLanguageToSupported(lang);
                UpdateUser(userId, new Dictionary<string, object> { { "primary_lang", supportedLang.ToString() } });
                return supportedLang;
            }

            return GlobalizationMethods.TryConvertLanguageToSupported(foundLang);
        }

        //specific to this methods
        private static SuniSupportedLanguages HandleUnknownUser(ulong userId, string lang, string userName, string avatar)
        {
            if (lang == null)
            {
                new DBMethods().InsertUser(userId, userName, avatar, commandNu: 1, primaryLang: SuniSupportedLanguages.FROM_CLIENT);
                return SuniSupportedLanguages.PT;
            }

            var supportedLang = GlobalizationMethods.TryConvertLanguageToSupported(lang);
            new DBMethods().InsertUser(userId, userName, avatar, commandNu: 1, primaryLang: supportedLang);
            return supportedLang;
        }
    }
}
