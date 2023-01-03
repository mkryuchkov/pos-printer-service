using System;
using System.Text;
using ESCPOS_NET.Utilities;
using mkryuchkov.ESCPOS.Goojprt;
using mkryuchkov.PosPrinter.Model.Core;

namespace mkryuchkov.PosPrinter.Service.Printing
{
    internal static class PrintQueryExtensions
    {
        private static readonly Encoding Cp866 =
            CodePagesEncodingProvider.Instance.GetEncoding(866);
        private static readonly Pt210 Emitter = new();

        public static byte[] GetPrintCommands(this PrintQuery<MessageInfo> query)
        {
            return ByteSplicer.Combine(
                Emitter.Initialize(),
                Emitter.Print($"From {query.Info.Author}\n  at {query.Info.Time.FormatTimeToMsk()}\n"),
                Emitter.FeedDots(5),
                query.Image != null
                    ? Emitter.PrintImage(query.Image)
                    : Array.Empty<byte>(),
                query.Text != null
                    ? Emitter.Print(query.Text, Cp866)
                    : Array.Empty<byte>(),
                Emitter.FeedLines(3),
                Emitter.Beep()
            );
        }

        // todo: parameters or localization
        private const string TimeFormat = "HH:mm:ss dd.MM.yyyy";
        private const string TimeZone = "Russian Standard Time";

        private static string FormatTimeToMsk(this DateTime time)
        {
            return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(time, TimeZone).ToString(TimeFormat);
        }
    }
}