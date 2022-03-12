using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ESCPOS_NET;
using ESCPOS_NET.Utilities;
using mkryuchkov.ESCPOS.Goojprt;

namespace pt_210_test
{
    class Program
    {
        private static readonly Encoding Cp866 =
            CodePagesEncodingProvider.Instance.GetEncoding(866);

        static async Task Main(string[] args)
        {
            Console.WriteLine("Start");

            IPrinter printer = OperatingSystem.IsLinux()
                ? new FilePrinter(filePath: "/dev/usb/lp0") // "/dev/rfcomm0"
                : new SerialPrinter("COM4", 9600);

            var emitter = new Pt210();
            printer.Write(ByteSplicer.Combine(
                emitter.Initialize(),
                emitter.Print("Hello! Привет!\n", Cp866),
                emitter.PrintImage(await File.ReadAllBytesAsync("./cube.jpg")),
                emitter.FeedLines(3),
                emitter.Beep(2)
            ));

            Console.WriteLine("End");
            await Task.Delay(5000);
            Console.WriteLine("Finish");
        }
    }
}