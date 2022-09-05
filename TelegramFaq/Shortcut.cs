using TelegramFaqBotHost.MongoModels;

namespace TelegramFaqBotHost.TelegramFaq
{
    public class Shortcut : MongoItem
    {
        public string Short { get; set; }
        public string Text { get; set; }

        public Shortcut() {}
        
        public Shortcut(string @short, string text)
        {
            Short = @short;
            Text = text;
        }
    }
}