using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types.InputFiles;

namespace mkryuchkov.PosPrinter.TgBot.Configuration
{
    public class ConfigureWebHook : IHostedService
    {
        private readonly ILogger<ConfigureWebHook> _logger;
        private readonly IServiceProvider _services;
        private readonly BotConfig _config;

        public ConfigureWebHook(
            ILogger<ConfigureWebHook> logger,
            IServiceProvider serviceProvider,
            IOptions<BotConfig> options)
        {
            _logger = logger;
            _services = serviceProvider;
            _config = options.Value;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _services.CreateScope();
            var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

            var webhookAddress = @$"{_config.Host}/bot/{_config.Token}";
            _logger.LogInformation("Setting webhook: {0}", webhookAddress);

            InputFileStream certInput = null;
            try
            {
                var cert = File.OpenRead(_config.CertPath);
                certInput = new InputFileStream(cert);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Certificate is missing: {0}", ex.Message);
            }
            finally
            {
                await botClient.SetWebhookAsync(
                    webhookAddress,
                    certInput,
                    cancellationToken: cancellationToken);

                if (certInput?.Content != null)
                {
                    await certInput.Content.DisposeAsync();
                }
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            using var scope = _services.CreateScope();
            var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

            _logger.LogInformation("Removing webhook");
            await botClient.DeleteWebhookAsync(cancellationToken: cancellationToken);
        }
    }
}