using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Lime.Protocol.Server;
using Take.Blip.Client;
using Lime.Protocol.Serialization;
using System.Reflection;
using Take.Blip.Client.Session;
using Lime.Protocol;

namespace event_bot
{
    /// <summary>
    /// Defines a type that is called once during the application initialization.
    /// </summary>
    public class Startup : IStartable
    {
        private readonly ISender _sender;
        private readonly Settings _settings;
        private readonly IStateManager _stateManager;

        public Startup(ISender sender, Settings settings, IStateManager stateManager)
        {
            _sender = sender;
            _settings = settings;
            _stateManager = stateManager;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _stateManager.SetStateAsync(Identity.Parse("1690059274398055@messenger.gw.msging.net"), "default", cancellationToken);
            TypeUtil.RegisterDocuments(typeof(Startup).GetTypeInfo().Assembly);
            return Task.CompletedTask;
        }
    }
}
