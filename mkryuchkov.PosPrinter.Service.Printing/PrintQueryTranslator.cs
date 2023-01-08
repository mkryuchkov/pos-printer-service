using System;
using System.Text;
using ESCPOS_NET.Utilities;
using Microsoft.Extensions.Options;
using mkryuchkov.ESCPOS.Goojprt;
using mkryuchkov.PosPrinter.Localization;
using mkryuchkov.PosPrinter.Model.Core;
using mkryuchkov.PosPrinter.Service.Printing.Configuration;
using mkryuchkov.PosPrinter.Service.Printing.Interfaces;

namespace mkryuchkov.PosPrinter.Service.Printing;

public sealed class PrintQueryTranslator : IPrintQueryTranslator
{
    private static readonly Encoding Cp866 = CodePagesEncodingProvider.Instance.GetEncoding(866)!;
    private static readonly Pt210 Emitter = new();

    private readonly PrinterConfig _config;
    private readonly ISharedStringLocalizer _localizer;

    public PrintQueryTranslator(
        IOptions<PrinterConfig> config,
        ISharedStringLocalizer localizer)
    {
        _config = config.Value;
        _localizer = localizer;
    }

    public byte[] GetPrintCommands(PrintQuery<MessageInfo> query)
    {
        _localizer.SetCurrentCultures(query.Info!.LanguageCode);

        return ByteSplicer.Combine(
            Emitter.Initialize(),
            Emitter.Print(GetHeader(query.Info!), Cp866),
            Emitter.FeedDots(10),
            GetImageCommands(query.Image, query.Caption),
            query.Text != null
                ? Emitter.Print(query.Text, Cp866, true)
                : Array.Empty<byte>(),
            Emitter.FeedLines(3),
            Emitter.Beep()
        );
    }

    private string GetHeader(MessageInfo info)
    {
        return _localizer[null, "HeaderWithFormat", info.Author!, FormatTimeToMsk(info.Time)];
    }

    private string FormatTimeToMsk(DateTime time)
    {
        var dt = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(time, _config.TimeZone!);
        return $"{dt.ToShortTimeString()} {dt.ToShortDateString()}";
    }

    private static byte[] GetImageCommands(byte[]? image, string? caption)
    {
        return ByteSplicer.Combine(
            ConditionalEmmit(image != null,
                () => Emitter.PrintImage(image)),
            ConditionalEmmit(image != null && caption != null,
                () => Emitter.Print(caption, Cp866, true)),
            ConditionalEmmit(image != null || caption != null,
                () => Emitter.FeedDots(10))
        );
    }

    private static byte[] ConditionalEmmit(bool condition, Func<byte[]> emitter)
    {
        return condition
            ? emitter()
            : Array.Empty<byte>();
    }
}