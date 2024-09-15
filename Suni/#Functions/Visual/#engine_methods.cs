using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace SunImageModels
{
    internal partial class Basics
    {
        internal static async Task<Image<Rgba32>> getRgba32FromUrl(string url)
        {
            using var client = new HttpClient();
            var stream = await client.GetStreamAsync(url);
            var image = await Image.LoadAsync<Rgba32>(stream);
            return image;
        }

        internal static Image<Rgba32> CircleFromOthers(Image<Rgba32> image, int wantedSize)
        {
            image.Mutate(x => x.Resize(wantedSize, wantedSize));
            using (var mask = new Image<Rgba32>(250, 250))
            {
                mask.Mutate(ctx => ctx.Fill(SixLabors.ImageSharp.Color.White, new SixLabors.ImageSharp.Drawing.EllipsePolygon(wantedSize / 2f, wantedSize / 2f, Math.Min(wantedSize, wantedSize) / 2f)));
                image.Mutate(ctx => ctx.SetGraphicsOptions(new GraphicsOptions { AlphaCompositionMode = PixelAlphaCompositionMode.DestIn })
                            .DrawImage(mask, new SixLabors.ImageSharp.Point(0, 0), 1));
            }
            return image;
        }

        internal static async Task<MemoryStream> ToStream(Image<Rgba32> imageToStream)
        {
            var resultadoStream = new MemoryStream();
            await imageToStream.SaveAsPngAsync(resultadoStream);
            resultadoStream.Seek(0, SeekOrigin.Begin);
            return resultadoStream;
        }
    }
}