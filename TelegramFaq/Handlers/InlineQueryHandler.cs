using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using TelegramFaqBotHost.MongoModels;
using TgBotFramework;

namespace TelegramFaqBotHost.TelegramFaq.Handlers
{
    public class InlineQueryHandler : IUpdateHandler<TelegramFaQContext>
    {
        private readonly DocAnchors _anchors;
        //private readonly MongoCrud<Shortcut> _shortcuts;
        private readonly ILogger<InlineQueryHandler> _logger;

        public InlineQueryHandler(DocAnchors anchors, ILogger<InlineQueryHandler> logger)
        {
            _anchors = anchors;
            _logger = logger;
        }

        public async Task HandleAsync(TelegramFaQContext context, UpdateDelegate<TelegramFaQContext> next, CancellationToken cancellationToken)
        {
            var queryString = context.Update.InlineQuery.Query.Replace(" ", "").ToLower();

            if (string.IsNullOrEmpty(queryString))
            {
                List<Shortcut> shortcuts = Data.Shortcuts;
                var inlineQueryResultBases = new List<InlineQueryResult>();
                foreach (var shortcut in shortcuts)
                {
                    inlineQueryResultBases.Add(new InlineQueryResultArticle(shortcuts.IndexOf(shortcut).ToString() , shortcut.Short, new InputTextMessageContent("📑 "+shortcut.Text) { DisableWebPagePreview = true, ParseMode = ParseMode.Markdown }) );
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
                    foreach (var anchor in _anchors.Anchors.Where(x => x.StartsWith(queryString)).Take(30))
                    {
                        var message = "https://core.telegram.org/bots/api#" + anchor;
                        inlineQueryResultBases.Add(new InlineQueryResultArticle(anchor, anchor,
                            new InputTextMessageContent("https://core.telegram.org/bots/api#" + anchor)
                                { DisableWebPagePreview = true, ParseMode = ParseMode.Markdown }));
                    }
                }
                else
                {
                    foreach (var anchor in _anchors.Anchors.Where(x=>x.Contains(queryString)).Take(30))
                    {
                        var librarySearchResult = GetLibraryEntity(anchor);
                        var message = $"{librarySearchResult}\nOfficial telegram api: [link](https://core.telegram.org/bots/api#{anchor})";
                        
                        inlineQueryResultBases.Add(new InlineQueryResultArticle(anchor, anchor,
                            new InputTextMessageContent(message) { DisableWebPagePreview = true, ParseMode = ParseMode.Markdown} ) {  } );
                    }
                }
                try
                {
                    await context.Client.AnswerInlineQueryAsync(context.Update.InlineQuery.Id, inlineQueryResultBases, cacheTime: 0, cancellationToken: cancellationToken);
                }
                catch (Exception e)
                {
                    _logger.LogError(context.Update.InlineQuery.Query);
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        public string GetLibraryEntity(string anchor)
        {
            var methods = GetAllByReflection.GetAllMethods();
            var classes = GetAllByReflection.GetAllClasses();
            
            var strictClass = GetStrictClassResult(anchor, classes);
            if (!string.IsNullOrWhiteSpace(strictClass))
            {
                return 
                    $"Library class name: [{strictClass}](https://github.com/search?q=repo%3ATelegramBots%2FTelegram.Bot+{strictClass}+language%3AC%23&type=code&l=C%23)\n" +
                    $"Search in examples: [link](https://github.com/search?q=repo%3ATelegramBots%2FTelegram.Bot.Examples%20{strictClass}&type=code)\n"
                ;
            }
            
            var strictMethod = GetStrictMethodResult(anchor, methods);
            if (strictMethod is not null)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"*Usage: *`await client.{strictMethod.Name}(...)`");
                if (!string.IsNullOrWhiteSpace(strictMethod.Description))
                {
                    sb.AppendLine($"_{strictMethod.Description}_");
                }

                sb.AppendLine();
                sb.AppendLine("Parameters:");
                foreach (KeyValuePair<string,string> param in strictMethod.Params)
                {
                    sb.AppendLine($"-`{param.Key}` : {param.Value}");
                }

                sb.AppendLine();
                if (!string.IsNullOrWhiteSpace(strictMethod.Returns))
                {
                    sb.AppendLine($"*Returns*: _{strictMethod.Returns}_");
                }

                sb.AppendLine();
                sb.AppendLine("*Github search:*");
                sb.AppendLine($"[Library](https://github.com/search?q=repo%3ATelegramBots%2FTelegram.Bot+{strictMethod.Name}+language%3AC%23&type=code&l=C%23)");
                sb.AppendLine(
                    $"[Examples](https://github.com/search?q=repo%3ATelegramBots%2FTelegram.Bot.Examples%20{strictMethod.Name}&type=code)");
                return sb.ToString();
                /*
                    $"\n" +
                    $"" +
                    $"\nGITHUB search:\n" 
                    // + $"Library method name:\n[{strictMethod}](https://github.com/search?q=repo%3ATelegramBots%2FTelegram.Bot+{strictMethod.Split('(')[0]}+language%3AC%23&type=code&l=C%23)\n" +
                   // $"Code search in examples: [link](https://github.com/search?q=repo%3ATelegramBots%2FTelegram.Bot.Examples%20{strictMethod.Split('(')[0]}&type=code)\n"
                ;*/
            }
            
            return "";
        }

        public MethodDescription GetStrictMethodResult(string anchor, List<MethodDescription> methods)
        {
            if (anchor.ToLower() == "sendmessage")
            {
                anchor = "SendTextMessage";
            }
            return methods.FirstOrDefault(x=>x.Name.ToLower().StartsWith(anchor.ToLower()+"async"));
        }

        public string GetStrictClassResult(string anchor, List<string> classes)
        {
            return classes.FirstOrDefault(x => string.Equals(x, anchor, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}