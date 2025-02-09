namespace Suni.Suni.NptEnvironment.Data.Types;

/// <summary>
/// Represents a Discord User in NPT environment.
/// </summary>
public class NptUser : SType
{
    private readonly DiscordUser _value;
    public NptUser(DiscordUser value) => _value = value;
    public override STypes Type => STypes.User;
    public override object Value => _value;

    [ExposedProperty("username")]
    public NptStr Username() => new NptStr(_value.Username);
    [ExposedProperty("avatarUrl")]
    public NptStr AvatarUrl() => new NptStr(_value.AvatarUrl);
}
