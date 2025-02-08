using System.IO;
using Newtonsoft.Json;
using RestSharp;
using Suni.Suni.Configuration.Interfaces;

namespace Suni.Suni.Functions
{
    //bruh
    public partial class Functions
    {
        public static async Task<(MemoryStream, string)> CalculateExpression(string exp, IAppConfig config)
        {
            var client = new RestClient(config.BaseUrlApi);
            var request = new RestRequest($"/calc/{exp}", Method.Get);
            byte[] response = await client.DownloadDataAsync(request);
            if (response == null || response.Length == 0)
            {
                var erroredImg = await Visual.Basics.ErroredImage();
                return (erroredImg, "failed to conect with api. Trying to solve this calc in basic calculator:\n:x: not implemented");
            }

            var memoryStream = new MemoryStream(response);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return (memoryStream, "");
        }

        public static async Task<string> GetPeople(IAppConfig config, string people, bool onlySays = true)
        {
            //add /says if onlySays is true
            string route = $"/people/{people}" + (onlySays ? "/says" : "");
            var client = new RestClient(config.BaseUrlApi);
            var request = new RestRequest(route, Method.Get);
            var response = await client.ExecuteAsync(request);
            if (response.IsSuccessful || response.Content != null)
            {
                var jsonResponse = JsonConvert.DeserializeObject<dynamic>(response.Content);
                string responseText = jsonResponse?.response?.text;
                //string avatarUrl = jsonResponse?.response?.avatar;
                return responseText ?? "?";
            }
            else
                return "??";
        }
    }
}

