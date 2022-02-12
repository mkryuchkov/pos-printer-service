using System;
using System.IO;
using System.Threading.Tasks;
using ESCPOS_NET;
using ESCPOS_NET.Utilities;

namespace pt_210_test
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Start");

            IPrinter printer = OperatingSystem.IsLinux()
                ? new FilePrinter(filePath: "/dev/usb/lp0") // "/dev/rfcomm0"
                : new SerialPrinter("COM3", 9600);

            var emitter = new Pt210();
            printer.Write(ByteSplicer.Combine(
                emitter.Initialize(),
                emitter.Print("Hello!\n"),
                emitter.PrintImage(await File.ReadAllBytesAsync("./cube.jpg")),
                emitter.FeedLines(3)
            ));

            Console.WriteLine("End");
            await Task.Delay(5000);
            Console.WriteLine("Finish");
        }
    }
}