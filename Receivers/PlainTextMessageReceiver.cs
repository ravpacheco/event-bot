using System;
using System.Threading;
using System.Threading.Tasks;
using Lime.Protocol;
using System.Diagnostics;
using Take.Blip.Client;
using event_bot.Services;

namespace event_bot.Receivers
{
    /// <summary>
    /// Defines a class for handling messages. 
    /// This type must be registered in the application.json file in the 'messageReceivers' section.
    /// </summary>
    public class PlainTextMessageReceiver : IMessageReceiver
    {
        private readonly ISender _sender;
        private readonly Settings _settings;
        private readonly IStateProcessorService _stateProcessorService;

        public PlainTextMessageReceiver(ISender sender, Settings settings, IStateProcessorService stateProcessorService)
        {
            _sender = sender;
            _settings = settings;
            _stateProcessorService = stateProcessorService;
        }

        public async Task ReceiveAsync(Message message, CancellationToken cancellationToken)
        {
            Trace.TraceInformation($"From: {message.From} \tContent: {message.Content}");
            await _sender.SendMessageAsync("Pong!", message.From, cancellationToken);
        }
    }
}
