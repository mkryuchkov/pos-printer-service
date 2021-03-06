using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Dithering;

namespace mkryuchkov.ESCPOS.Goojprt
{
    public static class ImageSharpExtensions
    {
        public static byte[] ToSingleBitPixelByteArray(
            this Image<Rgba32> image,
            int? maxWidth = null, int? maxHeight = null,
            IDither ditherAlg = null)
        {
            image.Mutate(operation =>
            {
                if (maxWidth.HasValue || maxHeight.HasValue)
                {
                    operation.Resize(new ResizeOptions
                    {
                        Mode = ResizeMode.Max,
                        Size = new Size(maxWidth ?? int.MaxValue, maxHeight ?? int.MaxValue),
                    });
                }

                operation
                    .BackgroundColor(Color.White)
                    .BinaryDither(ditherAlg ?? KnownDitherings.Sierra2);
            });

            return image.PackToBitmapArray();
        }


        private static byte[] PackToBitmapArray(this Image<Rgba32> image)
        {
            var bytesPerRow = (image.Width + 7 & -8) / 8;

            var result = new byte[bytesPerRow * image.Height];
            for (var y = 0; y < image.Height; y++)
            {
                var row = image.GetPixelRowSpan(y);
                var rowStartPosition = y * bytesPerRow;

                for (var x = 0; x < row.Length; x++)
                {
                    if (!row[x].IsBlack())
                    {
                        continue;
                    }

                    result[rowStartPosition + x / 8] |= (byte) (0x01 << (7 - (x % 8)));
                }
            }

            return result;
        }

        private static bool IsBlack(this Rgba32 rgba)
        {
            return rgba.R == 0 && rgba.G == 0 && rgba.B == 0;
        }
    }
}