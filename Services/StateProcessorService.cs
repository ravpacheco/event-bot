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
using Take.Blip.Client.Session;

namespace event_bot.Services
{
    public class StateProcessorService : IStateProcessorService
    {
        private readonly ISender _sender;
        private readonly IStateManager _stateManager;
        private readonly IBucketExtension _bucketExtension;

        public StateProcessorService(ISender sender, IStateManager stateManager, IBucketExtension bucketExtension)
        {
            _sender = sender;
            _stateManager = stateManager;
            _bucketExtension = bucketExtension;
        }
        public async Task ProcessState(Trigger trigger, Node fromNode, CancellationToken cancellationToken = default(CancellationToken))
        {
            var currentContext = await _bucketExtension.GetAsync<JsonDocument>($"{fromNode.ToIdentity()}:context", cancellationToken) ?? GetInitialContext();

            switch (trigger.StateId)
            {
                case "welcome-seeing":
                    await _sender.SendMessageAsync(new PlainText { Text = $"Eu também! 👀" }, fromNode, cancellationToken);

                    await Task.Delay(2000);

                    var quickReply = new Select
                    {
                        Text = $"Essas opções te ajudam ter uma conversa mais objetiva. Você pode falar, mas o quanto mais direto ao ponto, melhor 🎯",
                        Scope = SelectScope.Immediate,
                        Options = new SelectOption[]
                        {
                            new SelectOption
                            {
                                Order = 0,
                                Text = "OK! 🎯",
                                Value = new Trigger { StateId = "welcome-ok" }
                            }
                        }
                    };

                    await _sender.SendMessageAsync(quickReply, fromNode, cancellationToken);

                    break;
                case "welcome-ok":

                    await _sender.SendMessageAsync(new PlainText { Text = $"Falando em direto ao ponto.. 🎯" }, fromNode, cancellationToken);

                    await Task.Delay(2000);

                    quickReply = new Select
                    {
                        Text = $"Gostaria de receber avisos, desafios e slides das apresentações por aqui? ⬇️",
                        Scope = SelectScope.Immediate,
                        Options = new SelectOption[]
                        {
                            new SelectOption
                            {
                                Order = 0,
                                Text = "Sim",
                                Value = new Trigger { StateId = "welcome-notification-yes" }
                            },
                            new SelectOption
                            {
                                Order = 1,
                                Text = "Não",
                                Value = new Trigger { StateId = "welcome-notification-no" }
                            }
                        }
                    };

                    await _sender.SendMessageAsync(quickReply, fromNode, cancellationToken);

                    break;
                case "welcome-notification-yes":

                    currentContext["receiveAlert"] = true;

                    await _sender.SendMessageAsync(new PlainText { Text = $"Beleza! Vou te enviar os slides (pdf) por aqui. 😊" }, fromNode, cancellationToken);

                    await Task.Delay(2000);

                    var onboardingQuickReply = new Select
                    {
                        Text = $"Gostaria de conhecer o espaço do evento e um pouco do que preparamos para você?",
                        Scope = SelectScope.Immediate,
                        Options = new SelectOption[]
                        {
                            new SelectOption
                            {
                                Order = 0,
                                Text = "Sim",
                                Value = new Trigger { StateId = "onboarding-yes" }
                            },
                            new SelectOption
                            {
                                Order = 1,
                                Text = "Não",
                                Value = new Trigger { StateId = "onboarding-no" }
                            }
                        }
                    };

                    await _sender.SendMessageAsync(onboardingQuickReply, fromNode, cancellationToken);

                    break;
                case "welcome-notification-no":

                    currentContext["receiveAlert"] = false;

                    await _sender.SendMessageAsync(new PlainText { Text = $"Sem problemas! 😅" }, fromNode, cancellationToken);

                    await Task.Delay(2000);

                    onboardingQuickReply = new Select
                    {
                        Text = $"Gostaria de conhecer o espaço do evento e um pouco do que preparamos para você?",
                        Scope = SelectScope.Immediate,
                        Options = new SelectOption[]
                        {
                            new SelectOption
                            {
                                Order = 0,
                                Text = "Sim",
                                Value = new Trigger { StateId = "onboarding-yes" }
                            },
                            new SelectOption
                            {
                                Order = 1,
                                Text = "Não",
                                Value = new Trigger { StateId = "onboarding-no" }
                            }
                        }
                    };

                    await _sender.SendMessageAsync(onboardingQuickReply, fromNode, cancellationToken);

                    break;
                case "onboarding-yes":

                    await _sender.SendMessageAsync(new PlainText { Text = $"Confira algumas dicas ➡️" }, fromNode, cancellationToken);

                    await Task.Delay(2000);

                    var carouselTips = new DocumentCollection
                    {
                        ItemType = DocumentSelect.MediaType,
                        Items = new DocumentSelect[4]
                        {
                            new DocumentSelect
                            {
                                Header = new DocumentContainer
                                {
                                    Value = new MediaLink
                                    {
                                        Uri = new Uri("https://s3.amazonaws.com/elasticbeanstalk-us-east-1-405747350567/c4d/localiza%C3%A7%C3%A3o-18.png"),
                                        Type = new MediaType("image", "png"),
                                        Title = "Todo o espaço oferece experiências.",
                                        Text = "Veja o mapa no seu crachá 🗺️"
                                    }
                                },
                                Options = new DocumentSelectOption[]{}
                            },
                            new DocumentSelect
                            {
                                Header = new DocumentContainer
                                {
                                    Value = new MediaLink
                                    {
                                        Uri = new Uri("https://s3.amazonaws.com/elasticbeanstalk-us-east-1-405747350567/c4d/messenger+code-17.png"),
                                        Type = new MediaType("image", "png"),
                                        Title = "Ganhe brindes! Basta escanear imagens parecidas com essa 📱",
                                        Text = "Dica: use o Messenger 😉"
                                    }
                                },
                                Options = new DocumentSelectOption[]{}
                            },
                            new DocumentSelect
                            {
                                Header = new DocumentContainer
                                {
                                    Value = new MediaLink
                                    {
                                        Uri = new Uri("https://s3.amazonaws.com/elasticbeanstalk-us-east-1-405747350567/c4d/camisa.png"),
                                        Type = new MediaType("image", "png"),
                                        Title = "Precisando de ajuda?",
                                        Text = "Procure por alguém usando essa camisa 👕 ",
                                    }
                                },
                                Options = new DocumentSelectOption[]{}
                            },
                            new DocumentSelect
                            {
                                Header = new DocumentContainer
                                {
                                    Value = new MediaLink
                                    {
                                        Uri = new Uri("https://s3.amazonaws.com/elasticbeanstalk-us-east-1-405747350567/c4d/freeBeer.png"),
                                        Type = new MediaType("image", "png"),
                                        Title = "Fique para nossa festa no final evento.",
                                        Text = "A cerveja é por nossa conta 🍻",
                                    }
                                },
                                Options = new DocumentSelectOption[]{}
                            }
                        }
                    };

                    await _sender.SendMessageAsync(carouselTips, fromNode, cancellationToken);
                    await Task.Delay(6000);

                    var moreAboutEventMenu = new Select
                    {
                        Text = $"Saiba mais sobre o evento:",
                        Scope = SelectScope.Immediate,
                        Options = new SelectOption[]
                        {
                            new SelectOption
                            {
                                Order = 0,
                                Text = "Ver programação 📅",
                                Value = new Trigger { StateId = "menu-scheduler" }
                            },
                            new SelectOption
                            {
                                Order = 1,
                                Text = "Chatbot4Devs 🤖",
                                Value = new Trigger { StateId = "menu" }
                            }
                        }
                    };

                    await _sender.SendMessageAsync(moreAboutEventMenu, fromNode, cancellationToken);

                    break;
                case "onboarding-no":

                    var menu = new Select
                    {
                        Text = $"Ok!Esperamos que você curta essa imersão com chatbots.Qualquer coisa, tô aqui 😉",
                        Scope = SelectScope.Immediate,
                        Options = new SelectOption[]
                       {
                            new SelectOption
                            {
                                Order = 0,
                                Text = "Chatbot4Devs 🤖",
                                Value = new Trigger { StateId = "menu" }
                            }
                       }
                    };

                    await _sender.SendMessageAsync(menu, fromNode, cancellationToken);

                    break;
                case "menu":

                    await _sender.SendMessageAsync(new PlainText { Text = $"Veja essas opções ➡️" }, fromNode, cancellationToken);

                    await Task.Delay(2000);

                    var carouselMenu = new DocumentCollection
                    {
                        ItemType = DocumentSelect.MediaType,
                        Items = new DocumentSelect[3]
                        {
                            new DocumentSelect
                            {
                                Header = new DocumentContainer
                                {
                                    Value = new MediaLink
                                    {
                                        Uri = new Uri("https://s3.amazonaws.com/elasticbeanstalk-us-east-1-405747350567/c4d/chatbot4devs.png"),
                                        Type = new MediaType("image", "png"),
                                        Title = "Chatbots4Devs",
                                        Text = "Entenda nosso evento e veja como ter a melhoror experiência."
                                    }
                                },
                                Options = new DocumentSelectOption[]
                                {
                                    new DocumentSelectOption
                                    {
                                        Label = new DocumentContainer{ Value = "🤖 Conhecer evento" },
                                        Value = new DocumentContainer{ Value = new Trigger{ StateId = "menu" } }
                                    }
                                }
                            },
                            new DocumentSelect
                            {
                                Header = new DocumentContainer
                                {
                                    Value = new MediaLink
                                    {
                                        Uri = new Uri("https://s3.amazonaws.com/elasticbeanstalk-us-east-1-405747350567/c4d/programa%C3%A7%C3%A3o.png"),
                                        Type = new MediaType("image", "png"),
                                        Title = "Programação e Palestrantes",
                                        Text = "Confira o conteúdo de todas as palestras do evento."
                                    }
                                },
                                Options = new DocumentSelectOption[]
                                {
                                    new DocumentSelectOption
                                    {
                                        Label = new DocumentContainer{ Value = "📆 Ver programação" },
                                        Value = new DocumentContainer{ Value = new Trigger{ StateId = "menu-scheduler" } }
                                    }
                                }
                            },
                            new DocumentSelect
                            {
                                Header = new DocumentContainer
                                {
                                    Value = new MediaLink
                                    {
                                        Uri = new Uri("https://s3.amazonaws.com/elasticbeanstalk-us-east-1-405747350567/c4d/notifica%C3%A7%C3%B5es.png"),
                                        Type = new MediaType("image", "png"),
                                        Title = "Notificações e avisos",
                                        Text = "Recebimento de notificações.",
                                    }
                                },
                                Options = new DocumentSelectOption[]
                                {
                                    new DocumentSelectOption
                                    {
                                        Label = new DocumentContainer{ Value = "⚙ Configurar" },
                                        Value = new DocumentContainer{ Value = new Trigger{ StateId = "menu-settings" } }
                                    }
                                }
                            }
                        }
                    };

                    await _sender.SendMessageAsync(carouselMenu, fromNode, cancellationToken);
                    await Task.Delay(6000);

                    break;
                case "menu-event":

                    await _sender.SendMessageAsync(new PlainText { Text = $"Criamos o evento para unir pessoas que desenvolvem e influenciam várias áreas de empreendedorismo e tech." }, fromNode, cancellationToken);

                    await Task.Delay(2000);

                    var menuEvent = new Select
                    {
                        Text = $"Assim, o Chatbot4Devs trouxe conteúdos e interações no espaço do Sebrae MG para quem deseja saber tudo sobre chatbots.",
                        Options = new SelectOption[]
                        {
                            new SelectOption
                            {
                                Order = 0,
                                Text = "Conhecer mais",
                                Value = new WebLink { Uri = new Uri("http://chatbot4devs.take.net/") }
                            },
                            new SelectOption
                            {
                                Order = 1,
                                Text = "Menu Inicial",
                                Value = new Trigger { StateId = "menu" }
                            }
                        }
                    };

                    await _sender.SendMessageAsync(menuEvent, fromNode, cancellationToken);

                    break;
                case "menu-scheduler":

                    await _sender.SendMessageAsync(new PlainText { Text = $"Olha só o pessoal que vai palestrar:" }, fromNode, cancellationToken);

                    await Task.Delay(2000);

                    var speakersMenu = new DocumentCollection
                    {
                        ItemType = DocumentSelect.MediaType,
                        Items = new DocumentSelect[2]
                        {
                            new DocumentSelect
                            {
                                Header = new DocumentContainer
                                {
                                    Value = new MediaLink
                                    {
                                        Uri = new Uri("https://s3.amazonaws.com/elasticbeanstalk-us-east-1-405747350567/c4d/Rafael.png"),
                                        Type = new MediaType("image", "png"),
                                        Title = "⏰ 10h00 | Desenvolvimento de chatbots com a plataforma BLiP",
                                    }
                                },
                                Options = new DocumentSelectOption[]
                                {
                                }
                            },
                            new DocumentSelect
                            {
                                Header = new DocumentContainer
                                {
                                    Value = new MediaLink
                                    {
                                        Uri = new Uri("https://s3.amazonaws.com/elasticbeanstalk-us-east-1-405747350567/c4d/Caio.png"),
                                        Type = new MediaType("image", "png"),
                                        Title = "⏰ 12h00 | Como desenhar conversas com chatbots?",
                                    }
                                },
                                Options = new DocumentSelectOption[]
                                {
                                }
                            }
                        }
                    };

                    await _sender.SendMessageAsync(speakersMenu, fromNode, cancellationToken);
                    await Task.Delay(2000);

                    var menuSchedule = new Select
                    {
                        Text = $"Confira nossa agenda ➡️",
                        Scope = SelectScope.Immediate,
                        Options = new SelectOption[]
                        {
                            new SelectOption
                            {
                                Order = 0,
                                Text = "Chatbot4Devs 🤖",
                                Value = new Trigger { StateId = "menu" }
                            }
                        }
                    };

                    await _sender.SendMessageAsync(menuSchedule, fromNode, cancellationToken);

                    break;
                case "menu-settings":

                    var receiveAlert = bool.Parse(currentContext["receiveAlert"].ToString());

                    string text;
                    string optionText;
                    var optionPayload = new JsonDocument();

                    if (receiveAlert)
                    {
                        text = "${contact.name}, você está recebendo avisos, lembretes e slides 🔔";
                        optionText = "Não receber 🔕";
                        optionPayload["mute"] = true;
                    }
                    else
                    {
                        text = "${contact.name}, você não está recebendo avisos, lembretes e slides 🔕";
                        optionText = "Receber 🔔";
                        optionPayload["mute"] = false;
                    }

                    var menuAlert = new Select
                    {
                        Text = text,
                        Scope = SelectScope.Immediate,
                        Options = new SelectOption[]
                        {
                            new SelectOption
                            {
                                Text = optionText,
                                Order = 0,
                                Value = new Trigger { StateId = "menu-settings-update", Payload = optionPayload }
                            },
                            new SelectOption
                            {
                                Order = 0,
                                Text = "Chatbot4Devs 🤖",
                                Value = new Trigger { StateId = "menu" }
                            }
                        }
                    };

                    await _sender.SendMessageAsync(menuAlert, fromNode, cancellationToken);

                    break;
                case "menu-settings-update":

                    var mute = bool.Parse(trigger.Payload["mute"].ToString());
                    currentContext["receiveAlert"] = !mute;

                    await _sender.SendMessageAsync(new PlainText { Text = $"Combinado 😉" }, fromNode, cancellationToken);

                    await Task.Delay(2000);

                    menuAlert = new Select
                    {
                        Text = "Espero que esteja curtindo o evento 🤖",
                        Scope = SelectScope.Immediate,
                        Options = new SelectOption[]
                        {
                            new SelectOption
                            {
                                Order = 0,
                                Text = "Chatbot4Devs 🤖",
                                Value = new Trigger { StateId = "menu" }
                            }
                        }
                    };

                    await _sender.SendMessageAsync(menuAlert, fromNode, cancellationToken);

                    break;

                case "speaker1":
                    break;
                case "speaker2":
                    break;
                case "qrCode":

                    var hasScanned = currentContext.ContainsKey("hasScanned");

                    if (hasScanned)
                    {
                        var gif = new MediaLink
                        {
                            Uri = new Uri("https://s3.amazonaws.com/elasticbeanstalk-us-east-1-405747350567/c4d/brindeErro3-Shaq.gif"),
                            Type = new MediaType("image", "gif")
                        };

                        await _sender.SendMessageAsync(gif, fromNode, cancellationToken);

                        await Task.Delay(2000);

                        var hasScannedMenu = new Select
                        {
                            Text = "Aparentemente você já ganhou seu prêmio. Já olhou nos outros estandes ? 😎",
                            Scope = SelectScope.Immediate,
                            Options = new SelectOption[]
                            {
                                new SelectOption
                                {
                                    Order = 0,
                                    Text = "Chatbot4Devs 🤖",
                                    Value = new Trigger { StateId = "menu" }
                                }
                            }
                        };

                        await _sender.SendMessageAsync(hasScannedMenu, fromNode, cancellationToken);

                    }
                    else
                    {
                        await _sender.SendMessageAsync(new PlainText { Text = "Hum… me parece que você ganhou um brinde 😁" }, fromNode, cancellationToken);

                        var img = new MediaLink
                        {
                            Uri = new Uri("https://s3.amazonaws.com/elasticbeanstalk-us-east-1-405747350567/c4d/brinde-camisa.png"),
                            Type = new MediaType("image", "png")
                        };

                        await _sender.SendMessageAsync(img, fromNode, cancellationToken);

                        await Task.Delay(2000);

                        var scannedMenu = new Select
                        {
                            Text = "Apresente essa imagem num estande próximo a você.",
                            Scope = SelectScope.Immediate,
                            Options = new SelectOption[]
                            {
                                new SelectOption
                                {
                                    Order = 0,
                                    Text = "Chatbot4Devs 🤖",
                                    Value = new Trigger { StateId = "menu" }
                                }
                            }
                        };

                        await _sender.SendMessageAsync(scannedMenu, fromNode, cancellationToken);

                    }

                    currentContext["hasScanned"] = true;
                    break;
            }

            //Update user state
            await _stateManager.SetStateAsync(fromNode.ToIdentity(), trigger.StateId, cancellationToken);

            //Update user context 
            await _bucketExtension.SetAsync($"{fromNode.ToIdentity()}:context", currentContext, cancellationToken: cancellationToken);
        }

        private JsonDocument GetInitialContext()
        {
            var contextDictionary = new Dictionary<string, object>();
            return new JsonDocument(contextDictionary, MediaType.ApplicationJson);
        }
    }

    public interface IStateProcessorService
    {
        Task ProcessState(Trigger trigger, Node fromNode, CancellationToken cancellationToken = default(CancellationToken));
    }
}
