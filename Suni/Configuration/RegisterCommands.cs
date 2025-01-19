using System.Collections.Generic;
using System.Reflection;

namespace Suni.Suni.Configuration;

/// <summary>
/// This class has the methods used to configure Suni commands.
/// </summary>
public class CommandsConfigs
{
    /// <summary>
    /// Registers commands from the provided CommandsExtension.
    /// </summary>
    public static void RegisterCommands(CommandsExtension extension)
    {
        var publicInteractionCommandTypes = PublicInteractionCommandTypes();
        var userInstallCommandTypes = UserInstallInteractionCommandTypes(publicInteractionCommandTypes);
        var guildInstallCommandTypes = GuildInstallInteractionCommandTypes(publicInteractionCommandTypes);
        
        extension.AddCommands(userInstallCommandTypes);
        extension.AddCommands(guildInstallCommandTypes);
    }
    
    private static List<Type> PublicInteractionCommandTypes()
    {
        return Assembly.GetExecutingAssembly().GetTypes().Where(t =>
            t.IsClass && t.Namespace is not null && t.Namespace.Contains("Suni.Suni.Commands") &&
            !t.IsNested).ToList();
    }
    
    private static List<Type> UserInstallInteractionCommandTypes(List<Type> publicInteractionCommandTypes)
    {
        List<Type> userInstallInteractionCommandTypes = [];
        foreach (var type in publicInteractionCommandTypes)
        {
            if (type.GetCustomAttributes(typeof(InteractionInstallTypeAttribute), true).FirstOrDefault() is InteractionInstallTypeAttribute installTypeAttribute
                && installTypeAttribute.InstallTypes.Contains(DiscordApplicationIntegrationType.UserInstall))
                userInstallInteractionCommandTypes.Add(type);
            else
            {
                var methods = type.GetMethods();
                foreach (var method in methods)
                {
                    if (method.GetCustomAttributes(typeof(InteractionInstallTypeAttribute), true).FirstOrDefault() is InteractionInstallTypeAttribute methodInstallTypeAttribute
                        && methodInstallTypeAttribute.InstallTypes.Contains(DiscordApplicationIntegrationType.UserInstall)){
                        userInstallInteractionCommandTypes.Add(type);
                        break;
                    }
                }
            }
        }
        return userInstallInteractionCommandTypes;
    }
    
    private static List<Type> GuildInstallInteractionCommandTypes(List<Type> publicInteractionCommandTypes)
    {
        List<Type> guildInstallInteractionCommandTypes = [];
        foreach (var type in publicInteractionCommandTypes)
        {
            if (type.GetCustomAttributes(typeof(InteractionInstallTypeAttribute), true).FirstOrDefault() is InteractionInstallTypeAttribute installTypeAttribute
                && installTypeAttribute.InstallTypes.Contains(DiscordApplicationIntegrationType.GuildInstall)
                && !installTypeAttribute.InstallTypes.Contains(DiscordApplicationIntegrationType.UserInstall))
                guildInstallInteractionCommandTypes.Add(type);
            else
            {
                var methods = type.GetMethods();
                foreach (var method in methods)
                {
                    if (method.GetCustomAttributes(typeof(InteractionInstallTypeAttribute), true).FirstOrDefault() is InteractionInstallTypeAttribute methodInstallTypeAttribute
                        && methodInstallTypeAttribute.InstallTypes.Contains(DiscordApplicationIntegrationType.GuildInstall)
                        && !methodInstallTypeAttribute.InstallTypes.Contains(DiscordApplicationIntegrationType.UserInstall)){
                        guildInstallInteractionCommandTypes.Add(type);
                        break;
                    }
                }
            }
        }
        
        return guildInstallInteractionCommandTypes;
    }
}
