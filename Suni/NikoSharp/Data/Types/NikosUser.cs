namespace Suni.Suni.NikoSharp.Data.Types;

/// <summary>
/// Represents a Discord User in NikoSharp environment.
/// </summary>
public class NikosUser : SType
{
    private DiscordUser _value;
    public NikosUser(DiscordUser value = null) => _value = value;
    public override STypes Type => STypes.User;
    public override object Value => _value;
    public override string ToString() => "user";

    
    [ExposedProperty("toStr")]
    public override NikosStr ToNikosStr() => new("user");
    [ExposedProperty("username")]
    public NikosStr Username() => new NikosStr(_value.Username);
    [ExposedProperty("userId")]
    public NikosInt UserId() => new NikosInt((long)_value.Id); //the value can be converted explicitly because a UserID (ulong) will never be greater than NikosInt (long)
    [ExposedProperty("avatarUrl")]
    public NikosStr AvatarUrl() => new NikosStr(_value.AvatarUrl);
}
