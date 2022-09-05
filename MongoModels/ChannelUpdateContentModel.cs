using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramFaqBotHost.MongoModels
{
    public class ChannelUpdateContentModel : MongoItem
    {
        public string MediaGroupId { get; set; }
        public string Caption { get; set; }
        public MessageEntity[] CaptionEntities { get; set; }
        
        public bool IsForwarded { get; set; }
        
        public ChatId From { get; set; }
        public MessageType Type { get; set; }

        public ChannelUpdateContentModel()
        {
        }

        public ChannelUpdateContentModel(Message message)
        {
            MediaGroupId = message.MediaGroupId;
            Caption = message.Caption;
            CaptionEntities = message.CaptionEntities;
            
            IsForwarded = message.ForwardSenderName != null || message.ForwardFrom != null ||
                          message.ForwardFromChat != null;

            if (IsForwarded)
            {
                From = message.ForwardFromChat?.Id ?? message.ForwardFrom?.Id;
            }

            Type = message.Type;

        }

    }
}