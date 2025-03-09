using System.IO;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
namespace Suni.Suni.Functions.Visual;

public partial class CreateImage
{
    public static async Task<MemoryStream> BuildProfileUI(string urlAvatar, string banner, string name, ulong money, List<string> badges)
    {
        var avatar = await Basics.getRgba32FromUrl(urlAvatar); avatar = Basics.CircleFromOthers(avatar, 250);
        using var background = Image.Load<Rgba32>($"./Assets/images/banners/{banner.Split('-')[1]}.png");
        using Image<Rgba32> style = Image.Load<Rgba32>($"./Assets/images/banners/{banner.Split('-')[0]}.png");
        switch (banner.Split('-')[0])
        {
            case "night" or "nightwhite" or "nightwhitesolid":
                background.Mutate(ctx =>
                {
                    ctx.DrawImage(style, new Point(0, 0), 1f); //colocar o estilo de perfil
                    ctx.DrawImage(Basics.CircleFromOthers(avatar, 200), new Point(30, 95), 1f);
                });

                //texts
                FontCollection fontCollection = new FontCollection();
                FontFamily fontFamily = fontCollection.Add("./Assets/fonts/Roboto-Light.ttf");
                FontFamily fontFamily2 = fontCollection.Add("./Assets/fonts/Roboto-Light.ttf");
                Font fonteGrande = fontFamily.CreateFont(60);
                Font fonteGrossa = fontFamily2.CreateFont(25);

                background.Mutate(ctx =>
                {
                    ctx.DrawText(
                    $"{name}",
                    fonteGrande, Color.White,
                    new PointF(25, 10));
                    //10, 20
                    ctx.DrawText($"Ampersands: {money}", fonteGrossa, Color.White, new PointF(14, 320));
                });
                break;
            default:
                return null;
        }
        Console.WriteLine($"banner: {banner}\nnome: {name}\n" +
                            $"money: {money}\nBadges: {string.Join(' ', badges)}"); //DEBUG
        return await Basics.ToStream(background);
    }
}
