namespace Suni.Suni.Translations;

public partial class SolveLang
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
            if (ctx.Guild is not null)
                await dbMethods.InsertServerAsync(ctx.Guild.Id, ctx.Guild.Name, ctx.Guild.IconUrl, ServerStatusTypes.client, "", "");

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
}
