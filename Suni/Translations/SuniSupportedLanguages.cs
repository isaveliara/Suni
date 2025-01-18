namespace Suni.Suni.Globalization
{
    public enum SuniSupportedLanguages{
        PT, EN, RU, ES_MX, FROM_CLIENT
    }
    public partial class GlobalizationMethods
    {
        public static SuniSupportedLanguages ParseToLanguageSupported(string lang)
        {
            if (string.IsNullOrWhiteSpace(lang))
                return SuniSupportedLanguages.PT; //default
            
            return lang.ToLower() switch
            {
                "pt" or "pt-br" => SuniSupportedLanguages.PT,
                "en" or "en-us" or "en-gb" => SuniSupportedLanguages.EN,
                "es" or "es-mx" => SuniSupportedLanguages.ES_MX,
                "ru" or "ru-ru" => SuniSupportedLanguages.RU,
                _ => SuniSupportedLanguages.PT //default
            };
        }
    }
}