using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lime.Protocol;
using Take.Blip.Client;
using event_bot.Services;
using event_bot.Models;

namespace event_bot.Receivers
{
    public class TriggerMessageReceiver : IMessageReceiver
    {
        private readonly ISender _sender;
        private readonly IStateProcessorService _stateProcessorService;

        public TriggerMessageReceiver(ISender sender, IStateProcessorService stateProcessorService)
        {
            _sender = sender;
            _stateProcessorService = stateProcessorService;
        }
        public async Task ReceiveAsync(Message message, CancellationToken cancellationToken)
        {
            var trigger = message.Content as Trigger;

            await _stateProcessorService.ProcessState(trigger, message.From, cancellationToken);
        }
    }
}
