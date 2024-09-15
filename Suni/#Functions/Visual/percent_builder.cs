/*
This file is responsible for building the visual of ship.
Expect:(avatar-url-user1, avatar-url-user2, percent)
    >*
*/

using System.Threading.Tasks;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace SunImageModels
{
    public partial class CreateImage
    {
        public static async Task<Image<Rgba32>> BuildShip(string url1, string url2, byte percent)
        {
            var avatar1 = await Basics.getRgba32FromUrl(url1); var avatar2 = await Basics.getRgba32FromUrl(url2);
            avatar1 = Basics.CircleFromOthers(avatar1, 250); avatar2 = Basics.CircleFromOthers(avatar2, 250);
            var result = Image.Load<Rgba32>("./-assets/images/shipBackGround.png");

            result.Mutate(ctx =>
            {
                ctx.DrawImage(avatar1, new Point(75, 75), 1f);
                ctx.DrawImage(avatar2, new Point(375, 75), 1f);
                ctx.Fill(Color.Black, new RectangleF(100, 350, 500, 25));
                ctx.Fill(Color.Red, new RectangleF(100, 350, (500 * (percent / 100f)), 25));
                
                var collection = new FontCollection();
                var family = collection.Add("./-assets/fonts/Roboto-Light.ttf");
                var font = family.CreateFont(27, FontStyle.Regular);

                ctx.DrawText($"{percent}%", font, Color.White, new PointF(100 + (500 * (percent / 100f))+5, 350));
            });
            return result;
        }
    }
}
