using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.InlineQueryResults;
using TelegramFaqBotHost.MongoModels;
using TgBotFramework;

namespace TelegramFaqBotHost.TelegramFaq.Handlers
{
    public class InlineQueryHandler : IUpdateHandler<TelegramFaQContext>
    {
        private readonly DocAnchors _anchors;
        private readonly MongoCrud<Shortcut> _shortcuts;

        public InlineQueryHandler(DocAnchors anchors, MongoCrud<Shortcut> shortcuts)
        {
            _anchors = anchors;
            _shortcuts = shortcuts;
        }

        public async Task HandleAsync(TelegramFaQContext context, UpdateDelegate<TelegramFaQContext> next, CancellationToken cancellationToken)
        {
            var queryString = context.Update.InlineQuery.Query.Replace(" ", "").ToLower();

            if (string.IsNullOrEmpty(queryString))
            {
                IEnumerable<Shortcut> shortcuts = await _shortcuts.GetAll();
                var inlineQueryResultBases = new List<InlineQueryResult>();
                foreach (var shortcut in shortcuts)
                {
                    inlineQueryResultBases.Add(new InlineQueryResultArticle(shortcut.Id, shortcut.Short, new InputTextMessageContent(shortcut.Text)));
                }
                await context.Client.AnswerInlineQueryAsync(context.Update.InlineQuery.Id, inlineQueryResultBases, cancellationToken: cancellationToken);
            }
            else
            {
                if (_anchors.Anchors == null)
                    await _anchors.RefreshAsync();
                
                var inlineQueryResultBases = new List<InlineQueryResult>();
                if (context.Update.InlineQuery.Query.Length < 3)
                {
                    foreach (var anchor in _anchors.Anchors.Where(x => x.StartsWith(queryString)))
                    {
                        inlineQueryResultBases.Add(new InlineQueryResultArticle(anchor, anchor,
                            new InputTextMessageContent("https://core.telegram.org/bots/api#"+anchor)));
                    }
                }
                else
                {
                    foreach (var anchor in _anchors.Anchors.Where(x=>x.Contains(queryString)))
                    {
                        inlineQueryResultBases.Add(new InlineQueryResultArticle(anchor, anchor,
                            new InputTextMessageContent("https://core.telegram.org/bots/api#"+anchor)));
                    }
                }
                await context.Client.AnswerInlineQueryAsync(context.Update.InlineQuery.Id, inlineQueryResultBases, cancellationToken: cancellationToken);
            }
        }
    }
}