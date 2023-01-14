using mkryuchkov.PosPrinter.Model.Core;

namespace mkryuchkov.PosPrinter.Service.Printing.Interfaces;

public interface IPrintQueryTranslator
{
    byte[] GetPrintCommands(PrintQuery<MessageInfo> query);
}