using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace mkryuchkov.TgBot.Controllers
{
    public class TgWebhookController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Post(
            [FromServices] ITgUpdateHandler updateHandler,
            [FromBody] Update update,
            CancellationToken cancellationToken)
        {
            if (update == null)
            {
                return BadRequest();
            }

            await updateHandler.Handle(update, cancellationToken).ConfigureAwait(false);

            return Ok();
        }
    }
}