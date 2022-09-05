using Microsoft.Extensions.Options;
using TgBotFramework;

namespace TelegramFaqBotHost.TelegramFaq
{
    public class TelegramFaqBot : BaseBot
    {
        public TelegramFaqBot(IOptions<BotSettings> options) : base(options)
        {
        }
    }
}