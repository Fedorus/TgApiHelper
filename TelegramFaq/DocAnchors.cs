using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace TelegramFaqBotHost.TelegramFaq
{
    public class DocAnchors
    {
        public List<string> Anchors { get; set; }

        public async Task RefreshAsync()
        {
            using (var httpClient = new HttpClient())
            {
                var site = await httpClient.GetStringAsync("https://core.telegram.org/bots/api");
                var anchors = site.Substrings("href=\"#", "\"").Distinct().ToList();
                anchors.Sort();
                Anchors = anchors;
            }
        }
    }
}