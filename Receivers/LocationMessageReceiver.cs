using System.Threading;
using System.Threading.Tasks;
using Lime.Protocol;
using Lime.Messaging.Contents;
using Take.Blip.Client;
using event_bot.Models;

namespace event_bot.Receivers
{
    public class LocationMessageReceiver : IMessageReceiver
    {
        private readonly ISender _sender;

        public LocationMessageReceiver(ISender sender)
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

            var location = new Location { Latitude = -19.936721211961, Longitude = -43.972370624542 };
            var text = new PlainText { Text = "Estamos aqui, vem pra cá 😍" };

            var quickReply = new Select
            {
                Text = "Acho que uma dessas opções pode te atender melhor 😉",
                Scope = SelectScope.Immediate,
                Options = new SelectOption[]
                        {
                            new SelectOption
                            {
                                Text = "Ver programação 📆",
                                Value = new Trigger { StateId = "M3" }
                            },
                            new SelectOption
                            {
                                Text = "Chatbot4Devs 🤖",
                                Value = new Trigger { StateId = "M1" }
                            }
                        }
            };

            await _sender.SendMessageAsync(text, message.From, cancellationToken);
            await _sender.SendMessageAsync(location, message.From,  cancellationToken);
            await Task.Delay(6000);
            await _sender.SendMessageAsync(quickReply, message.From, cancellationToken);
        }
    }
}
