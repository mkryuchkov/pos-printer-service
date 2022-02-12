using System;
using ESCPOS_NET.Emitters;
using ESCPOS_NET.Emitters.BaseCommandValues;
using ESCPOS_NET.Utilities;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing.Processors.Dithering;

namespace pt_210_test
{
    public sealed class Pt210 : EPSON
    {
        private const int MaxWidth = 384;

        public byte[] PrintImage(byte[] image, int maxWidth = MaxWidth, IDither dithering = null)
        {
            return ByteSplicer.Combine(BufferImage(image, maxWidth, dithering), Initialize());
        }

        private byte[] BufferImage(byte[] image, int? maxWidth = null, IDither dithering = null)
        {
            var result = new ByteArrayBuilder();

            int width;
            int height;
            byte[] imageData;
            using (var img = Image.Load(image))
            {
                imageData = img.ToSingleBitPixelByteArray(maxWidth, ditherAlg: dithering);
                height = img.Height;
                width = img.Width;
            }

            var heightL = (byte) height;
            var heightH = (byte) (height >> 8);
            var byteWidth = (width + 7 & -8) / 8; // packing dots into bytes
            var widthL = (byte) byteWidth;
            var widthH = (byte) (byteWidth >> 8);
            result.Append(
                new byte[] {Cmd.GS, Images.ImageCmdLegacy, 0x30, 0x00, widthL, widthH, heightL, heightH});

            result.Append(imageData);

            return result.ToArray();
        }

#pragma warning disable 0809
        private const string ForbiddenMethodMessage = "EPSON implementation is not allowed.";
        private static readonly MethodAccessException ForbiddenMethodException = new(ForbiddenMethodMessage);

        [Obsolete(ForbiddenMethodMessage)]
        public override byte[] PrintImage(byte[] image, bool isHiDpi, bool isLegacy = false, int maxWidth = -1,
            int color = 1)
        {
            throw ForbiddenMethodException;
        }

        [Obsolete(ForbiddenMethodMessage)]
        public override byte[] BufferImage(byte[] image, int maxWidth = -1, bool isLegacy = false, int color = 1)
        {
            throw ForbiddenMethodException;
        }
#pragma warning restore 0809
    }
}