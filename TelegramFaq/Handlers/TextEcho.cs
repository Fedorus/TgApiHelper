using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using TgBotFramework;

namespace TelegramFaqBotHost.TelegramFaq.Handlers
{
    public class TextEcho : IUpdateHandler<TelegramFaQContext>
    {
        public async Task HandleAsync(TelegramFaQContext context, UpdateDelegate<TelegramFaQContext> next, CancellationToken cancellationToken)
        {
            await context.Bot.Client.SendTextMessageAsync(context.Update.Message.Chat.Id,
                "With any questions connect to @Sinys", cancellationToken: cancellationToken);
        }
    }
}