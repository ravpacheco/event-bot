using System;
using SimpleInjector;
using Take.Blip.Client.Activation;
using event_bot.Services;

namespace event_bot
{
    public class ServiceProvider : Container, IServiceContainer
    {
        public ServiceProvider() => Initialize();

        public object GetService(Type serviceType)
            => GetInstance(serviceType);

        public void RegisterService(Type serviceType, object instance)
            => RegisterSingleton(serviceType, instance);

        public void RegisterService(Type serviceType, Func<object> instanceFactory)
            => RegisterSingleton(serviceType, instanceFactory);

        private void Initialize()
        {
            RegisterSingleton<IStateProcessorService, StateProcessorService>();
        }
    }
}
