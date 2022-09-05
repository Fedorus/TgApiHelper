using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TelegramFaqBotHost.MongoModels;
using TelegramFaqBotHost.TelegramFaq;
using TgBotFramework;
using TelegramFaqBotHost.TelegramFaq.Handlers;
    


namespace TelegramFaqBotHost
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; set; }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();
            services.Configure<MongoSettings>(Configuration.GetSection(nameof(TelegramFaqBot)));
            services.AddScoped<TelegramFaqBot>();
            services.AddSingleton<TextEcho>();
            services.AddSingleton<CommandsHandler>();
            services.AddScoped<ExceptionCatcher<TelegramFaqBot>>();
            services.AddSingleton<MongoCrud<Shortcut>>();
            services.AddSingleton<DocAnchors>();
            services.AddScoped<InlineQueryHandler>();
            services.AddScoped<LeaveChatHandler>();
            services.AddScoped<ChosenInlineResultHandler>();
            services.AddBotService<TelegramFaQContext>(Configuration.GetSection("TelegramFaqBot")["ApiToken"],
                builder => builder.UseLongPolling()
                    .SetPipeline(pipe => pipe.Use<ExceptionCatcher<TelegramFaqBot>>()
                        .MapWhen<InlineQueryHandler>(On.InlineQuery)
                        .MapWhen(In.PrivateChat, branch => branch
                            .UseWhen<CommandsHandler>(On.Message)
                            .Use<TextEcho>()
                        )
                        .MapWhen<LeaveChatHandler>(In.GroupChat) //auto-leave group chat 
                        .MapWhen<ChosenInlineResultHandler>(On.ChosenInlineResult)));
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var shortcuts =  app.ApplicationServices.GetService<MongoCrud<Shortcut>>();
            if (shortcuts != null && shortcuts.CountAsync().GetAwaiter().GetResult() == 0 && File.Exists("TelegramFaq/Shortcuts.json"))
            {
                var list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Shortcut>>(
                    File.ReadAllText("TelegramFaq/Shortcuts.json"));
                foreach (var shortcut in list)
                {
                    shortcuts.InsertAsync(shortcut).GetAwaiter().GetResult();
                }
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context => { await context.Response.WriteAsync("Hello World!"); });
            });
        }
    }

    public class ExceptionCatcher<T> : IUpdateHandler<TelegramFaQContext>
    {
        private readonly ILogger<ExceptionCatcher<T>> _logger;

        public ExceptionCatcher(ILogger<ExceptionCatcher<T>> logger)
        {
            _logger = logger;
        }
        public async Task HandleAsync(TelegramFaQContext context, UpdateDelegate<TelegramFaQContext> next, CancellationToken cancellationToken)
        {
            try
            {
                await next(context, cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[{Now}]", DateTime.Now);
            }
        }
    }
}