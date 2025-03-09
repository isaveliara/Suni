using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Suni.Suni.Functions.Visual;

public partial class CreateImage
{
    public static async Task<MemoryStream> BuildJooj(byte type, string urlImage)
    {
        var image = await Basics.getRgba32FromUrl(urlImage);
        int width = image.Width; int height = image.Height;

        using var resultImage = new Image<Rgba32>(width, height);
        var rect = new Rectangle(0, 0, width / 2, height);

        switch (type)
        {
            case 1://"left":jooj
                var leftHalf = image.Clone(ctx => ctx.Crop(rect));
                resultImage.Mutate(ctx => ctx.DrawImage(leftHalf, rect.Location, 1f));

                //flip the left half of the image horizontallyESQUERDA
                leftHalf.Mutate(ctx => ctx.Flip(FlipMode.Horizontal));

                //define a rectangle for the right half of the image
                var rightRect = new Rectangle(width / 2, 0, width / 2, height);

                //copy the flipped left half to the right side of the result image
                resultImage.Mutate(ctx => ctx.DrawImage(leftHalf, rightRect.Location, 1f));
                break;


            case 2://"right":ojjo
                var rightHalf = image.Clone(ctx => ctx.Crop(new Rectangle(width / 2, 0, width / 2, height)));
                resultImage.Mutate(ctx => ctx.DrawImage(rightHalf, new Point(width / 2, 0), 1f));

                rightHalf.Mutate(ctx => ctx.Flip(FlipMode.Horizontal));

                var leftRect = new Rectangle(0, 0, width / 2, height);

                resultImage.Mutate(ctx => ctx.DrawImage(rightHalf, leftRect.Location, 1f));
                break;


            case 3://"jojo":
                var upHalf = image.Clone(ctx => ctx.Crop(new Rectangle(width / 2, 0, width / 2, height)));
                resultImage.Mutate(ctx => ctx.DrawImage(upHalf, new Point(width / 2, 0), 1f));

                upHalf.Mutate(ctx => ctx.Flip(FlipMode.Vertical));

                var upRect = new Rectangle(0, 0, width / 2, height);

                resultImage.Mutate(ctx => ctx.DrawImage(upHalf, upRect.Location, 1f));
                break;


            case 4://"ojoj":
                var downHalf = image.Clone(ctx => ctx.Crop(new Rectangle(width / 2, 0, width / 2, height)));
                resultImage.Mutate(ctx => ctx.DrawImage(downHalf, new Point(width / 2, 0), 1f));

                downHalf.Mutate(ctx => ctx.Flip(FlipMode.Vertical));

                var downRect = new Rectangle(0, 0, width / 2, height);

                resultImage.Mutate(ctx => ctx.DrawImage(downHalf, downRect.Location, 1f));
                break;
            default:
                return null;
        }

        return await Basics.ToStream(resultImage);
    }
}
