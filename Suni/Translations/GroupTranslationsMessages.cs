using System.Collections;
using System.Globalization;
using System.Resources;

namespace Suni.Suni.Globalization;

public partial class SolveLang
{
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
}
