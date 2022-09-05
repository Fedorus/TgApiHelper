using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TgBotFramework;

namespace TelegramFaqBotHost.TelegramFaq.Handlers
{
    internal class CommandsHandler : IUpdateHandler<TelegramFaQContext>
    {
        private readonly ILogger<CommandsHandler> _logger;

        public CommandsHandler(ILogger<CommandsHandler> logger)
        {
            _logger = logger;
        }
        public async Task HandleAsync(TelegramFaQContext context, UpdateDelegate<TelegramFaQContext> next, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[{Now}] User {MessageFrom} called {MessageText}", DateTime.Now, context.Update.Message.From, context.Update.Message.Text);
        }
    }
}