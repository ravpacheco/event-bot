using event_bot.Models;
using Lime.Messaging.Contents;
using Lime.Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Take.Blip.Client;
using Take.Blip.Client.Extensions.Bucket;
using Take.Blip.Client.Extensions.Directory;
using Take.Blip.Client.Session;

namespace event_bot.Receivers
{
    public class FirstTextMessageReceiver : IMessageReceiver
    {
        private readonly ISender _sender;
        private readonly IStateManager _stateManager;
        private readonly IBucketExtension _bucketExtension;
        private readonly IDirectoryExtension _directoryExtension;

        public FirstTextMessageReceiver(ISender sender, IStateManager stateManager, IBucketExtension bucketExtension, IDirectoryExtension directoryExtension)
        {
            _sender = sender;
            _stateManager = stateManager;
            _bucketExtension = bucketExtension;
            _directoryExtension = directoryExtension;
        }
        public async Task ReceiveAsync(Message message, CancellationToken cancellationToken)
        {
            //Get user details
            var userDetails = await _directoryExtension.GetDirectoryAccountAsync(message.From, cancellationToken);

            //Update user state
            await _stateManager.SetStateAsync(message.From.ToIdentity(), "welcome", cancellationToken);

            //Create a user context
            var myContextKey = $"{message.From.ToIdentity()}:context";
            var contextDocument = new JsonDocument();
            await _bucketExtension.SetAsync(myContextKey, contextDocument);

            // Salutation texts
            var textDocument = new PlainText { Text = "Olá ${tudo bem|como vai|tudo certo}! Que bom ter você em nosso primeiro #Chatbot4Devs! Estamos muito felizes ☺️" };

            var salutationMessage = new Message
            {
                To = message.From,
                Content = textDocument,
                Id = EnvelopeId.NewId(),
                Metadata = new Dictionary<string, string>
                {
                    { "#message.replaceVariables", "true" }
                }
            };

            await _sender.SendMessageAsync(salutationMessage, cancellationToken);
            await Task.Delay(2000);

            textDocument.Text = "Estou aqui para facilitar sua interação com o evento 🤖";
            await _sender.SendMessageAsync(textDocument, message.From, cancellationToken);
            await Task.Delay(2000);

            // Start Options
            Select quickReply = new Select
            {
                Text = "Tá vendo esses botões azuis aqui embaixo? ⬇️",
                Scope = SelectScope.Immediate,
                Options = new SelectOption[]
                {
                    new SelectOption
                    {
                        Text = "Estou vendo 👀",
                        Value = new Trigger
                        {
                            StateId = "welcome-seeing"
                        }
                    }
                }
            };

            await _sender.SendMessageAsync(quickReply, message.From, cancellationToken);
        }
    }
}
