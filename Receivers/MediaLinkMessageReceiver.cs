using System;
using System.Threading;
using System.Threading.Tasks;
using Lime.Protocol;
using Lime.Messaging.Contents;
using Take.Blip.Client;
using event_bot.Models;

namespace event_bot.Receivers
{
    public class MediaLinkMessageReceiver : IMessageReceiver
    {
        private readonly ISender _sender;
        private readonly string smallThumbsUpUrl = "fbcdn.net/v/t39.1997-6/p100x100/851587_369239346556147_162929011_n.png?_nc_ad=z-m&oh=6c4deff5f1cf8c2f7940dab6f54862a2&oe=5A2DF5B0";
        private readonly string bigThumbsUpUrl = "fbcdn.net/v/t39.1997-6/851557_369239266556155_759568595_n.png?_nc_ad=z-m&oh=f41542005a7945be79a6181951f7a37b&oe=5A2703DC";

        public MediaLinkMessageReceiver(ISender sender)
        {
            _sender = sender;
        }

        public async Task ReceiveAsync(Message message, CancellationToken cancellationToken = default(CancellationToken))
        {
            var chatStateMessage = new Message
            {
                Content = new ChatState { State = ChatStateEvent.Composing },
                To = message.From
            };
            await _sender.SendMessageAsync(chatStateMessage, cancellationToken);

            var content = message.Content as MediaLink;
            var contentType = content.Type;
            Document media = null;

            if (contentType.Type.Equals("audio"))
            {
                media = new MediaLink
                {
                    Uri = new Uri("https://s3.amazonaws.com/elasticbeanstalk-us-east-1-405747350567/c4d/audio-1.mp3"),
                    Type = new MediaType("audio", "mp3")
                };
            }
            else if (contentType.Type.Equals("image"))
            {
                if (content.Uri.ToString().Trim().Contains(smallThumbsUpUrl) || content.Uri.ToString().Trim().Contains(bigThumbsUpUrl))
                {
                    media = new PlainText { Text = "👍" };
                }
                else
                {
                    media = new MediaLink
                    {
                        Uri = new Uri("https://s3.amazonaws.com/elasticbeanstalk-us-east-1-405747350567/c4d/no-understand-img.png"),
                        Type = new MediaType("image", "png")
                    };
                }
            }
            else if (contentType.Type.Equals("document") || contentType.Type.Equals("application"))
            {
                media = new MediaLink
                {
                    Uri = new Uri("https://s3.amazonaws.com/elasticbeanstalk-us-east-1-405747350567/c4d/no-understand-doc.png"),
                    Type = new MediaType("image", "png")
                };
            }
            else if (contentType.Type.Equals("video"))
            {
                media = new PlainText { Text = "🙈" };
            }

            var quickReply = new Select
            {
                Text = "Acho que uma dessas opções pode te atender melhor 😉",
                Scope = SelectScope.Immediate,
                Options = new SelectOption[]
                        {
                            new SelectOption
                            {
                                Text = "Ver programação 📆",
                                Value = new Trigger { StateId = "menu-scheduler" }
                            },
                            new SelectOption
                            {
                                Text = "Chatbot4Devs 🤖",
                                Value = new Trigger { StateId = "menu" }
                            }
                        }
            };



            await _sender.SendMessageAsync(media, message.From, cancellationToken);
            await Task.Delay(6000);
            await _sender.SendMessageAsync(quickReply, message.From, cancellationToken);
        }
    }
}
