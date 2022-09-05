using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TelegramFaqBotHost.MongoModels;
using TgBotFramework;

namespace TelegramFaqBotHost.TelegramFaq.Handlers
{
    public class ChosenInlineResultHandler : IUpdateHandler<TelegramFaQContext>
    {
        private readonly MongoCrud<Shortcut> _items;
        private readonly ILogger<ChosenInlineResultHandler> _logger;

        public ChosenInlineResultHandler(MongoCrud<Shortcut> items, ILogger<ChosenInlineResultHandler> logger)
        {
            _items = items;
            _logger = logger;
        }

        public async Task HandleAsync(TelegramFaQContext context, UpdateDelegate<TelegramFaQContext> next, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(context.Update.ChosenInlineResult.Query))
            {
                _logger.LogInformation("[{Now}] User {From} chosen {S}", 
                    DateTime.Now, context.Update.ChosenInlineResult.From, (await _items.GetAsync(context.Update.ChosenInlineResult.ResultId)).Short);
            }
            else 
                _logger.LogInformation("[{Now}] User {From} chosen {ResultId} {Query}", 
                DateTime.Now, context.Update.ChosenInlineResult.From, context.Update.ChosenInlineResult.ResultId, context.Update.ChosenInlineResult.Query);
        }
    }
}