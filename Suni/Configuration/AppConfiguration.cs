using Config.Net;
using Suni.Suni.Configuration.Interfaces;
namespace Suni.Suni.Configuration;

public class AppConfig
{

    public static IAppConfig NewAppConfig()
    {
        var config = new ConfigurationBuilder<IAppConfig>().UseDotEnvFile(".env").Build();
        return config;
    }
}