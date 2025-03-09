using Suni.Suni.NikoSharp.Data;
using MoonSharp.Interpreter;
namespace Suni.Suni.SuniApi;


[MoonSharpUserData]
public class SuniApi
{
    private readonly CommandContext _ctx;

    public SuniApi(CommandContext ctx) => _ctx = ctx;

    public DiscordGuild Guild => _ctx.Guild;
    public DiscordChannel Channel => _ctx.Channel;
    public DiscordUser User => _ctx.User;

    public async Task<Diagnostics> Message()
    {
        try
        {
            var message = await _ctx.GetResponseAsync();
            return message != null ? Diagnostics.Success : Diagnostics.InvalidItemException;
        }
        catch (Exception){
            return Diagnostics.RaisedException;
        }
    }

    public async Task<Diagnostics> SendMessage(string content, bool tts = false)
    {
        try
        {
            await _ctx.Channel.SendMessageAsync(new DiscordMessageBuilder().WithContent(content).WithTTS(tts));
            return Diagnostics.Success;
        }
        catch (Exception){
            return Diagnostics.RaisedException;
        }
    }

    public async Task<Diagnostics> SendEmbed(Action<SuniEmbedBuilder> embedBuilder)
    {
        try
        {
            var builder = new SuniEmbedBuilder();
            embedBuilder(builder);
            await _ctx.Channel.SendMessageAsync(builder.Build());
            return Diagnostics.Success;
        }
        catch (Exception){
            return Diagnostics.RaisedException;
        }
    }

    public async Task<Diagnostics> EditMessage(ulong messageId, string newContent)
    {
        try
        {
            var message = await _ctx.Channel.GetMessageAsync(messageId);
            if (message == null)
                return Diagnostics.InvalidItemException;

            await message.ModifyAsync(new DiscordMessageBuilder().WithContent(newContent));
            return Diagnostics.Success;
        }
        catch (Exception)
        {
            return Diagnostics.RaisedException;
        }
    }

    public async Task<Diagnostics> CreateChannel(string name, string type = "text", ulong? categoryId = null)
    {
        try
        {
            var channelType = type.ToLower() == "voice" ? DiscordChannelType.Voice : DiscordChannelType.Text;

            DiscordChannel parentChannel = null;
            if (categoryId.HasValue)
            {
                parentChannel = await _ctx.Guild.GetChannelAsync(categoryId.Value);
                if (parentChannel == null)
                    return Diagnostics.InvalidItemException;
            }

            await _ctx.Guild.CreateChannelAsync(name, channelType, parent: parentChannel);
            return Diagnostics.Success;
        }
        catch (Exception){
            return Diagnostics.RaisedException;
        }
    }

    public async Task<Diagnostics> DeleteChannel(ulong channelId)
    {
        try
        {
            var channel = await _ctx.Guild.GetChannelAsync(channelId);
            if (channel == null)
                return Diagnostics.InvalidItemException;

            await channel.DeleteAsync();
            return Diagnostics.Success;
        }
        catch (Exception){
            return Diagnostics.RaisedException;
        }
    }

    public async Task<Diagnostics> BanMember(ulong userId, string reason = "")
    {
        try
        {
            var member = await _ctx.Guild.GetMemberAsync(userId);
            if (member == null)
                return Diagnostics.InvalidItemException;

            await member.BanAsync(reason: reason);
            return Diagnostics.Success;
        }
        catch (Exception){
            return Diagnostics.RaisedException;
        }
    }

    public async Task<Diagnostics> AddRole(ulong userId, ulong roleId)
    {
        try
        {
            var member = await _ctx.Guild.GetMemberAsync(userId);
            var role = await _ctx.Guild.GetRoleAsync(roleId);
            if (role == null || member == null)
                return Diagnostics.InvalidItemException;

            await member.GrantRoleAsync(role);
            return Diagnostics.Success;
        }
        catch (Exception){
            return Diagnostics.RaisedException;
        }
    }

    public async Task<Diagnostics> ChangeNickname(ulong userId, string newNickname)
    {
        try
        {
            var member = await _ctx.Guild.GetMemberAsync(userId);
            if (member == null)
                return Diagnostics.InvalidItemException;

            await member.ModifyAsync(m => m.Nickname = newNickname);
            return Diagnostics.Success;
        }
        catch (Exception){
            return Diagnostics.RaisedException;
        }
    }

    public async Task<Diagnostics> CreateInvite(int maxUses = 0, int maxAge = 86400, bool temporary = false)
    {
        try
        {
            await _ctx.Channel.CreateInviteAsync(maxUses, maxAge, temporary);
            return Diagnostics.Success;
        }
        catch (Exception){
            return Diagnostics.RaisedException;
        }
    }

    public async Task<bool> HasPermission(ulong userId, DiscordPermission permission)
    {
        try
        {
            var member = await _ctx.Guild.GetMemberAsync(userId);
            return member.PermissionsIn(_ctx.Channel).HasPermission(permission);
        }
        catch (Exception){
            return false;
        }
    }

    public async Task<Diagnostics> AddReaction(ulong messageId, string emoji)
    {
        try
        {
            var message = await _ctx.Channel.GetMessageAsync(messageId);
            if (message == null)
                return Diagnostics.InvalidItemException;

            var emojiObj = DiscordEmoji.FromName(_ctx.Client, emoji);
            await message.CreateReactionAsync(emojiObj);
            return Diagnostics.Success;
        }
        catch (Exception){
            return Diagnostics.RaisedException;
        }
    }

    public async Task<Diagnostics> RemoveReaction(ulong messageId, ulong userId, string emoji)
    {
        try
        {
            var message = await _ctx.Channel.GetMessageAsync(messageId);
            if (message == null)
                return Diagnostics.InvalidItemException;

            var emojiObj = DiscordEmoji.FromName(_ctx.Client, emoji);
            await message.DeleteReactionAsync(emojiObj, await _ctx.Client.GetUserAsync(userId));
            return Diagnostics.Success;
        }
        catch (Exception){
            return Diagnostics.RaisedException;
        }
    }
}

[MoonSharpUserData]
public class SuniEmbedBuilder
{
    private readonly DiscordEmbedBuilder _builder = new();

    public SuniEmbedBuilder WithTitle(string title)
    {
        _builder.Title = title;
        return this;
    }

    public SuniEmbedBuilder WithDescription(string description)
    {
        _builder.Description = description;
        return this;
    }

    public SuniEmbedBuilder WithColor(int r, int g, int b)
    {
        _builder.Color = new DiscordColor(r, g, b);
        return this;
    }

    public SuniEmbedBuilder AddField(string name, string value, bool inline = false)
    {
        _builder.AddField(name, value, inline);
        return this;
    }

    public DiscordEmbed Build() => _builder.Build();
}

