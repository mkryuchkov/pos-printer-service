using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace mkryuchkov.PosPrinter.TgBot.Controllers
{
    public class TgWebhookController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Post(
            [FromServices] IPosPrinterBot posPrinterBot,
            [FromBody] Update update)
        {
            if (update == null)
            {
                return BadRequest();
            }

            await posPrinterBot.HandleTgUpdate(update);

            return Ok();
        }
    }
}