namespace Suni.Suni.NptEnvironment.Data.Types;

/// <summary>
/// Represents a Discord User in NPT environment.
/// </summary>
public class NptUser : SType
{
    private DiscordUser _value;
    public NptUser(DiscordUser value = null) => _value = value;
    public override STypes Type => STypes.User;
    public override object Value => _value;
    public override string ToString() => "user";

    
    [ExposedProperty("toStr")]
    public override NptStr ToNptStr() => new("user");
    [ExposedProperty("username")]
    public NptStr Username() => new NptStr(_value.Username);
    [ExposedProperty("userId")]
    public NptInt UserId() => new NptInt((long)_value.Id); //the value can be converted explicitly because a UserID (ulong) will never be greater than NptInt (long)
    [ExposedProperty("avatarUrl")]
    public NptStr AvatarUrl() => new NptStr(_value.AvatarUrl);
}
