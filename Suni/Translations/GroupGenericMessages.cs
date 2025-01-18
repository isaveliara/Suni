using System.Globalization;
using System.Resources;

namespace Suni.Suni.Translations;

public partial class SolveLang
{
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
