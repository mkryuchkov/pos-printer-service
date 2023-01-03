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

        public static byte[] GetPrintCommands<TId>(this IPrintQuery<TId> query)
        {
            return query.Type switch
            {
                PrintQueryType.Text => WrapCommand(Emitter.Print(query.Text, Cp866)),
                PrintQueryType.Image => WrapCommand(Emitter.PrintImage(query.Image)),
                _ => throw new ArgumentOutOfRangeException("Unsupported query type")
            };
        }

        private static byte[] WrapCommand(byte[] command)
        {
            return ByteSplicer.Combine(
                Emitter.Initialize(),
                command,
                Emitter.FeedLines(3),
                Emitter.Beep()
            );
        }
    }
}