using System;
using ESCPOS_NET;

namespace mkryuchkov.PosPrinter.Service.Printing.Configuration
{
    internal static class PrinterConfigExtensions
    {
        public static BasePrinter GetPrinter(this PrinterConfig config)
        {
            return config.Type switch
            {
                PrinterType.File => new FilePrinter(config.Location),
                PrinterType.Serial => new SerialPrinter(config.Location, config.BaudRate),
                _ => throw new ArgumentOutOfRangeException("Wrong printer type")
            };
        }
    }
}