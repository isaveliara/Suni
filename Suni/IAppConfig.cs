namespace Sun.Bot;

public interface IAppConfig
{
    string SuniToken { get; }
    string CanaryToken { get; }
    string BaseUrlApi { get; }
    string BaseUrl { get; }
    ulong SupportServerId { get; }
}
