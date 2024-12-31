namespace Sun.Globalization
{
    public enum SuniSupportedLanguages{
        PT, EN, RU, FROM_CLIENT
    }
    public partial class GlobalizationMethods
    {
        public static SuniSupportedLanguages TryConvertLanguageToSupported(string lang)
        {
            if (string.IsNullOrWhiteSpace(lang))
                return SuniSupportedLanguages.PT; //default
            
            return lang.ToLower() switch
            {
                "pt" or "pt-br" => SuniSupportedLanguages.PT,
                "en" or "en-us" or "en-gb" => SuniSupportedLanguages.EN,
                "ru" or "ru-ru" => SuniSupportedLanguages.RU,
                _ => SuniSupportedLanguages.PT //default
            };
        }
    }
}