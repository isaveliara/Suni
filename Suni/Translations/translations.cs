using System.Collections;
using System.Globalization;
using System.Resources;

namespace Sun.Globalization;

public class SolveLang
{
    public SuniSupportedLanguages Language { get; private set; }
    public GroupTranslationsMessages Commands { get; private set; }
    public GroupGenericMessages GenericMessages { get; private set; }
    
    private SolveLang(SuniSupportedLanguages language)
    {
        Language = language;
        Commands = new GroupTranslationsMessages(language.ToString());
        GenericMessages = new GroupGenericMessages(language.ToString());
    }

    /// <summary>Creates a new instance of SolveLang.</summary>
    public static async Task<SolveLang> SolveLangAsync(string lang = null, CommandContext ctx = null)
    {
        if (lang is null && ctx is null){
            Console.WriteLine("Error: Both lang and ctx parameters are null. Defaulting to PT.");
            return new SolveLang(SuniSupportedLanguages.PT);
        }

        var dbMethods = new DBMethods();
        SuniSupportedLanguages resolvedLang;

        if (ctx != null){
            //get language from database
            var dbLang = await dbMethods.GetUserPrimaryLangAsync(ctx.User.Id);

            if (dbLang == SuniSupportedLanguages.FROM_CLIENT){
                //if the language in the database is FROM_CLIENT or the user does not exist, insert it into the database
                dbMethods.InsertUser(
                    ctx.User.Id,
                    ctx.User.Username,
                    ctx.User.AvatarUrl,
                    primaryLang: GlobalizationMethods.ParseToLanguageSupported(lang ?? ctx.User.Locale)
                );

                //requeries the language after entering
                dbLang = await dbMethods.GetUserPrimaryLangAsync(ctx.User.Id);
            }

            if (!string.IsNullOrEmpty(ctx.User.Locale) || !string.IsNullOrEmpty(lang)){
                Console.WriteLine($"Resolving locale {lang ?? ctx.User.Locale} for user {ctx.User.Id} - translations.cs");
                var userLang = GlobalizationMethods.ParseToLanguageSupported(lang ?? ctx.User.Locale);
                if (dbLang == SuniSupportedLanguages.FROM_CLIENT){
                    await dbMethods.UpdateUserPrimaryLangAsync(ctx.User.Id, userLang);
                    resolvedLang = userLang;
                }
                else
                    resolvedLang = dbLang;
            }
            else
                resolvedLang = dbLang;
        }
        else
            //converts `lang` parameter to `SuniSupportedLanguages`
            resolvedLang = GlobalizationMethods.ParseToLanguageSupported(lang);

        return new SolveLang(resolvedLang);
    }

    public class GroupTranslationsMessages
    {
        private readonly ResourceManager _resourceManager;
        private readonly ResourceManager _baseResourceManager;
        private readonly CultureInfo _culture;

        public GroupTranslationsMessages(string language)
        {
            _resourceManager = new ResourceManager("Suni.Resources.Messages", typeof(GroupTranslationsMessages).Assembly);
            _baseResourceManager = new ResourceManager("Suni.Resources.Messages", typeof(GroupTranslationsMessages).Assembly);
            _culture = new CultureInfo(language);
        }

        public string GetString(string key)
            => _resourceManager.GetString(key, _culture) ?? _baseResourceManager.GetString(key);

        public double GetTranslationCoveragePercentage(){
            var baseResourceSet = _baseResourceManager.GetResourceSet(CultureInfo.InvariantCulture, true, true);
            var localizedResourceSet = _resourceManager.GetResourceSet(_culture, true, true);

            int totalKeys = baseResourceSet.Cast<DictionaryEntry>().Count();
            int matchingKeys = localizedResourceSet.Cast<DictionaryEntry>()
                                                    .Count(entry => baseResourceSet.Cast<DictionaryEntry>()
                                                                                    .Any(baseEntry => baseEntry.Key.Equals(entry.Key) ));//&& !string.IsNullOrEmpty(entry.Value?.ToString())

            return (double)matchingKeys / totalKeys * 100;
        }

        public (string message, string coupleName) GetShipMessages(int percent, string user1, string user2){
            string name = string.Format(GetString("Ship_Response"), 
                            user1.Substring(0, user1.Length / 2) + user2.Substring(user2.Length / 2));

            string message = percent switch{
                0 => "...",
                <= 13 => GetString("Ship_13"),
                <= 24 => GetString("Ship_24"),
                <= 49 => GetString("Ship_49"),
                <= 69 => GetString("Ship_69"),
                <= 89 => new Random().Next(1, 3) == 1
                    ? string.Format(GetString("Ship_89_1"), user1, user2)
                    : string.Format(GetString("Ship_89_2"), user2, user1),
                90 => GetString("Ship_90"),
                _ => "?"
            };

            return (message, name);
        }

        public (string message_error, string embedTitle, string embedDescription, string content, string noMoney, string success) GetMarryMessages(byte error_id, string ctxUserMention, string userMention)
        {
            return (
                GetString($"Marry_E{error_id}"), //0-3 errors
                GetString("Marry_embedTitle"),
                string.Format(GetString("Marry_embedDescription"), ctxUserMention),
                string.Format(GetString("Marry_messageContent"), userMention),
                string.Format(GetString("Marry_noMoney"), ctxUserMention, userMention),
                string.Format(GetString("Marry_messageContent_OnSuccess"), ctxUserMention, userMention)
            );
        }
    }

    public class GroupGenericMessages
    {
        private readonly ResourceManager _resourceManager;
        private readonly ResourceManager _baseResourceManager;
        private readonly CultureInfo _culture;

        public GroupGenericMessages(string language){
            _resourceManager = new ResourceManager("Suni.Resources.GenericMessages", typeof(GroupGenericMessages).Assembly);
            _baseResourceManager = new ResourceManager("Suni.Resources.GenericMessages", typeof(GroupGenericMessages).Assembly);
            _culture = new CultureInfo(language);
        }

        private string GetStringGeneric(string key)
            => _resourceManager.GetString(key, _culture) ?? _baseResourceManager.GetString(key);

        /// <summary>Message: No resources to analyze here! </summary>
        public string ErrNoResources
            => GetStringGeneric("noResources");

        /// <summary>Message: :x: | You do not have sufficient permissions to execute this action! </summary>
        public string ErrUnauthorized
            => GetStringGeneric("failUnauthorized");

        /// <summary>Message: :x: An unknown error occurred... </summary>
        public string ErrUnknown
            => GetStringGeneric("unknownError");

        /// <summary>Message: :x: An internal error occurred... </summary>
        public string ErrInternal
            => GetStringGeneric("internalError");

        /// <summary>Message: No response within the expected time! </summary>
        public string ErrTimeExpired
            => GetStringGeneric("failTime");
        
    }
}
