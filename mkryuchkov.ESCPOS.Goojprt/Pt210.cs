using System;
using System.Text;
using ESCPOS_NET.Emitters;
using ESCPOS_NET.Emitters.BaseCommandValues;
using ESCPOS_NET.Utilities;
using mkryuchkov.WordWrap;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing.Processors.Dithering;

namespace mkryuchkov.ESCPOS.Goojprt
{
    /// <summary>
    /// GoojPrt PT-210 printer command emitter.
    /// </summary>
    public sealed class Pt210 : EPSON
    {
        private const int MaxWidth = 384;
        private const int MaxLineWidth = 32;

        /// <summary>
        /// Beep <paramref name="count"/> times.
        /// </summary>
        /// <param name="count">Times to beep.</param>
        /// <returns>Printer command set.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> out of range.</exception>
        public byte[] Beep(byte count = 1)
        {
            if (count is < 0 or > 10)
            {
                throw new ArgumentOutOfRangeException($"Count ({count}) must be in the range of 1 to 10.");
            }

            return new byte[] { Cmd.ESC, 0x42, count, 0x04 };
        }

        /// <summary>
        /// Print text with specified encoding.
        /// </summary>
        /// <param name="data">Text.</param>
        /// <param name="encoding">Encoding.</param>
        /// <param name="wrap">Wrap words or not.</param>
        /// <returns>Printer command set.</returns>
        public byte[] Print(string data, Encoding encoding, bool wrap = false)
        {
            return (encoding ?? Encoding.Default).GetBytes(wrap ? data.Wrap(MaxLineWidth) : data);
        }

        /// <summary>
        /// Print text with default encoding.
        /// </summary>
        /// <param name="data">Text.</param>
        /// <returns>Printer command set.</returns>
        public override byte[] Print(string data)
        {
            return Print(data, Encoding.Default);
        }

        /// <summary>
        /// Print image.
        /// </summary>
        /// <param name="image">Image itself.</param>
        /// <param name="maxWidth">Max printing width in dots.</param>
        /// <param name="dithering">Dithering algorithm.</param>
        /// <returns>Printer command set.</returns>
        public byte[] PrintImage(byte[] image, int maxWidth = MaxWidth, IDither dithering = null)
        {
            return ByteSplicer.Combine(BufferImage(image, maxWidth, dithering), Initialize());
        }

        private static byte[] BufferImage(byte[] image, int? maxWidth = null, IDither dithering = null)
        {
            var result = new ByteArrayBuilder();

            int width;
            int height;
            byte[] imageData;
            using (var img = Image.Load<Rgba32>(image))
            {
                imageData = img.ToSingleBitPixelByteArray(maxWidth, ditherAlg: dithering);
                height = img.Height;
                width = img.Width;
            }

            var heightL = (byte)height;
            var heightH = (byte)(height >> 8);
            var byteWidth = (width + 7 & -8) / 8; // packing dots into bytes
            var widthL = (byte)byteWidth;
            var widthH = (byte)(byteWidth >> 8);
            result.Append(
                new byte[] { Cmd.GS, Images.ImageCmdLegacy, 0x30, 0x00, widthL, widthH, heightL, heightH });

            result.Append(imageData);

            return result.ToArray();
        }

        /// <summary>
        /// Overridden <see cref="EPSON"/> method.
        /// Calls <see cref="Pt210.PrintImage(byte[],int,IDither)"/> with default params.
        /// It is recommended to use <see cref="Pt210.PrintImage(byte[],int,IDither)"/> method instead.
        /// </summary>
        /// <param name="image">Image itself.</param>
        /// <param name="isHiDpi">Not used.</param>
        /// <param name="isLegacy">Not used.</param>
        /// <param name="maxWidth">Default value is used.</param>
        /// <param name="color">Not used.</param>
        /// <returns>Command to print the given image.</returns>
        public override byte[] PrintImage(byte[] image, bool isHiDpi, bool isLegacy = false, int maxWidth = -1,
            int color = 1)
        {
            return PrintImage(image);
        }

        /// <summary>
        /// Overridden <see cref="EPSON"/> method.
        /// Calls <see cref="Pt210.BufferImage(byte[],int?,IDither)"/> with default params.
        /// It is recommended to use <see cref="Pt210.BufferImage(byte[],int?,IDither)"/> method instead.
        /// </summary>
        /// <param name="image">Image itself.</param>
        /// <param name="maxWidth">Default value is used.</param>
        /// <param name="isLegacy">Not used.</param>
        /// <param name="color">Not used.</param>
        /// <returns></returns>
        public override byte[] BufferImage(byte[] image, int maxWidth = -1, bool isLegacy = false, int color = 1)
        {
            return BufferImage(image, maxWidth);
        }
    }
}