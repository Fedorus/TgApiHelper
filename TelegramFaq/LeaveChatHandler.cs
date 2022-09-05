using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using TgBotFramework;

namespace TelegramFaqBotHost.TelegramFaq;

public class LeaveChatHandler : IUpdateHandler<TelegramFaQContext>
{
    public async Task HandleAsync(TelegramFaQContext context, UpdateDelegate<TelegramFaQContext> next, CancellationToken cancellationToken)
    {
        await context.Client.LeaveChatAsync(context.Update.Message.Chat.Id, cancellationToken);
    }
}