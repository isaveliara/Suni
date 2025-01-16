using DSharpPlus.Net.Gateway;
namespace Suni.Suni.Controllers;

public class GatewayController : IGatewayController
{

    public async Task HeartbeatedAsync(IGatewayClient client)
    {
        await Task.CompletedTask;
        return;
    }
    public async Task ResumeAttemptedAsync(IGatewayClient _)
    {
        await Task.CompletedTask;
    }
    public async Task ZombiedAsync(IGatewayClient _)
    {
        await Task.CompletedTask;
    }
    public async Task ReconnectRequestedAsync(IGatewayClient _)
    {
        await Task.CompletedTask;
    }
    public async Task ReconnectFailedAsync(IGatewayClient _)
    {
        await Task.CompletedTask;
    }
    public async Task SessionInvalidatedAsync(IGatewayClient _)
    {
        await Task.CompletedTask;
    }

}