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
            return query.Type switch
            {
                PrintQueryType.Text => WrapCommand(Emitter.Print(query.Text, Cp866), query.Info),
                PrintQueryType.Image => WrapCommand(Emitter.PrintImage(query.Image), query.Info),
                _ => throw new ArgumentOutOfRangeException("Unsupported print type")
            };
        }

        private const string TimeFormat = "HH:mm:ss dd.MM.yyyy";

        private static byte[] WrapCommand(byte[] command, MessageInfo info)
        {
            return ByteSplicer.Combine(
                Emitter.Initialize(),
                Emitter.Print($"From {info.Author}\n  at {info.Time.ToString(TimeFormat)}\n"),
                command,
                Emitter.FeedLines(3),
                Emitter.Beep()
            );
        }
    }
}