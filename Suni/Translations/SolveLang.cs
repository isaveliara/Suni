namespace Sun.Globalization;

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
}
