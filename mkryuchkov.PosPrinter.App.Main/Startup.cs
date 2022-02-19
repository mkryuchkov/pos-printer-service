using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using mkryuchkov.PosPrinter.TgBot.Configuration;
using mkryuchkov.PosPrinter.TgBot.Controllers;

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
            services.ConfigureTgBot(Configuration);
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