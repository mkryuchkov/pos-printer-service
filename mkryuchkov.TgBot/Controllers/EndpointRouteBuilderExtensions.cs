using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using mkryuchkov.TgBot.Configuration;

namespace mkryuchkov.TgBot.Controllers
{
    public static class EndpointRouteBuilderExtensions
    {
        public static ControllerActionEndpointConventionBuilder MapWebhookRoute(
            this IEndpointRouteBuilder endpoints,
            BotConfig botConfig)
        {
            return endpoints.MapControllerRoute(
                Const.TgWebHook,
                $"bot/{botConfig.Token}",
                new
                {
                    controller = Const.TgWebHook,
                    action = "Post"
                });
        }
    }
}