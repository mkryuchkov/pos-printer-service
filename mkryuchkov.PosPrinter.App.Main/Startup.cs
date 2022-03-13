using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using mkryuchkov.PosPrinter.Model.Core;
using mkryuchkov.PosPrinter.Service.Core;
using mkryuchkov.PosPrinter.Service.Queue;
using mkryuchkov.PosPrinter.Service.TgBot;
using mkryuchkov.TgBot;
using mkryuchkov.TgBot.Configuration;
using mkryuchkov.TgBot.Controllers;

namespace mkryuchkov.PosPrinter.App.Main
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSingleton<IQueue<IPrintQuery<int>>, PrintQueryQueue>()
                .AddSingleton<IQueue<IPrintResult<int>>, PrintResultQueue>()
                .AddHostedService<PrintService.PrintService>()
                .ConfigureTgBot(Configuration)
                .AddSingleton<ITgUpdateHandler, PosPrinterBot>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IOptions<BotConfig> botConfig)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseCors();
            app.UseEndpoints(endpoints => endpoints.MapWebhookRoute(botConfig.Value));
        }
    }
}