using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;
using RestSharp;

namespace HandlerFunctions.Listeners
{
    public class RECEIVE
    {
        internal static async Task Role(GuildMemberUpdatedEventArgs e)
        {
            var client = new RestClient(new SunBot.DotenvItems().BaseUrlApi);
            var request = new RestRequest($"/listeners/{e.Guild.Id}/receive/role", Method.Get);
            var response = await client.PostAsync(request);
            Console.WriteLine(response.Content);
        }
    }
    public class TAKE
    {
        internal static async Task Role(GuildMemberUpdatedEventArgs e)
        {
            var client = new RestClient(new SunBot.DotenvItems().BaseUrlApi);
            var request = new RestRequest($"/listeners/{e.Guild.Id}/take/role", Method.Get);
            var response = await client.PostAsync(request);
            Console.WriteLine(response.Content);
        }
    }
}