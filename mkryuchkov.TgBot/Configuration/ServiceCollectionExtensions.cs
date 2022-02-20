using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

namespace mkryuchkov.TgBot.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureTgBot(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<BotConfig>(
                configuration.GetSection(nameof(BotConfig)));

            services.AddHostedService<ConfigureWebHook>();

            services.AddHttpClient(Const.TgWebHook)
                .AddTypedClient<ITelegramBotClient>(httpClient =>
                    new TelegramBotClient(
                        configuration[$"{nameof(BotConfig)}:Token"],
                        httpClient));

            services
                .AddControllers()
                .AddNewtonsoftJson();

            return services;
        }
    }
}