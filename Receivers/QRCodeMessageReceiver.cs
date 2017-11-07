using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Lime.Protocol;
using Lime.Messaging.Contents;
using Take.Blip.Client;
using Take.Blip.Client.Extensions.Bucket;
using event_bot.Services;
using event_bot.Models;

namespace event_bot.Receivers
{
    public class QRCodeMessageReceiver : IMessageReceiver
    {
        private readonly IBucketExtension _bucketExtension;
        private readonly IStateProcessorService _stateProcessorService;
        private readonly ISender _sender;

        public QRCodeMessageReceiver(ISender sender, IBucketExtension bucketExtension, IStateProcessorService stateProcessorService)
        {
            _sender = sender;
            _bucketExtension = bucketExtension;
            _stateProcessorService = stateProcessorService;
        }

        public async Task ReceiveAsync(Message message, CancellationToken cancellationToken = default(CancellationToken))
        {
            var chatStateMessage = new Message
            {
                Content = new ChatState { State = ChatStateEvent.Composing },
                To = message.From
            };
            await _sender.SendMessageAsync(chatStateMessage, cancellationToken);

            var chatState = message.Content as ChatState;

            await _stateProcessorService.ProcessState(new Trigger { StateId = "qrCode" }, message.From, cancellationToken);
        }

        private JsonDocument GetInitialContext()
        {
            var contextDictionary = new Dictionary<string, object>();
            return new JsonDocument(contextDictionary, MediaType.ApplicationJson);
        }
    }
}
