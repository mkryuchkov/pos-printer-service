using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using mkryuchkov.PosPrinter.Localization;
using mkryuchkov.PosPrinter.Model.Core;
using mkryuchkov.PosPrinter.Service.Core;
using mkryuchkov.PosPrinter.Service.Printing;
using mkryuchkov.PosPrinter.Service.Printing.Configuration;
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
            services.AddSharedLocalization();

            services.AddSingleton<IQueue<PrintQuery<MessageInfo>>, PrintQueryQueue>();
            services.AddSingleton<IQueue<PrintResult<MessageInfo>>, PrintResultQueue>();
            services.AddQueryPrinting(Configuration);
            services.AddQueueHandler<PrintQuery<MessageInfo>, PrintQueryHandler>();
            services.AddQueueHandler<PrintResult<MessageInfo>, PrintResultHandler>();

            services.AddTgBot(Configuration);
            services.AddSingleton<ITgUpdateHandler, TgUpdateHandler>();
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