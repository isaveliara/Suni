/*
File responsible for generate the messages/other things to be used by suni.
The translation sys. could be here, but I got lazy and decided to wait until square cloud BD support is ready.
*/

using System;
using System.IO;
using System.Threading.Tasks;
using RestSharp;

namespace SunFunctions
{
    //bruh
    public partial class Functions
    {
        public static async Task<(MemoryStream, string)> calculateExpression(string exp)
        {
            var client = new RestClient(new SunBot.DotenvItems().BaseUrlApi);
            var request = new RestRequest($"/api/calc/{exp}", Method.Get);
            byte[] response = await client.DownloadDataAsync(request);
            if (response == null || response.Length == 0)
            {
                var  erroredImg = await SunImageModels.Basics.ErroredImage();
                return (erroredImg, "failed to conect with api. Trying to solve this calc in basic form:\n:x: not implemented");
            }
            
            var memoryStream = new MemoryStream(response);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return (memoryStream, "");
        }
    }
}

