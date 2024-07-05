using System.Collections.Generic;

namespace TelegramFaqBotHost.TelegramFaq;

public class MethodDescription
{
    public string Name { get; set; }
    public string MethodFullDesc { get; set; }
    public Dictionary<string, string> Params { get; set; } = new Dictionary<string, string>();
    public string Description { get; set; }
    public string Returns { get; set; }
}