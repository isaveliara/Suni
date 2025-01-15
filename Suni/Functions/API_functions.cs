using System.IO;
using Newtonsoft.Json;
using RestSharp;

namespace Sun.Functions
{
    //bruh
    public partial class Functions
    {
        public static async Task<(MemoryStream, string)> CalculateExpression(string exp)
        {
            var client = new RestClient(new Sun.Bot.DotenvItems().BaseUrlApi);
            var request = new RestRequest($"/calc/{exp}", Method.Get);
            byte[] response = await client.DownloadDataAsync(request);
            if (response == null || response.Length == 0)
            {
                var  erroredImg = await Sun.ImageModels.Basics.ErroredImage();
                return (erroredImg, "failed to conect with api. Trying to solve this calc in basic form calculator:\n:x: not implemented");
            }
            
            var memoryStream = new MemoryStream(response);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return (memoryStream, "");
        }

        public static async Task<string> GetPeople(string people, bool onlySays = true)
        {
            //add /says if onlySays is true
            string route = $"/people/{people}" + (onlySays ? "/says" : "");
            var client = new RestClient(new Sun.Bot.DotenvItems().BaseUrlApi);
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

