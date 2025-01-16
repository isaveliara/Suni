namespace Suni.Suni.Configuration.Interfaces;

public interface IAppConfig
{
    string SuniToken { get; }
    string CanaryToken { get; }
    string BaseUrlApi { get; }
    string BaseUrl { get; }
    ulong SupportServerId { get; }
}
