using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.CoreLib.EF.Events
{
    public class EventHandlerContainer
    {
        private IServiceProvider _serviceProvider = null;
        private static Dictionary<string, List<Type>> _mappings = new Dictionary<string, List<Type>>();

        public EventHandlerContainer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public static void Subscribe<T, THandler>()
         where T : EventBase
         where THandler : IEventHandler<T>
        {
            var name = typeof(T).Name;

            if (!_mappings.ContainsKey(name))
            {
                _mappings.Add(name, new List<Type> { });
            }

            _mappings[name].Add(typeof(THandler));
        }

        public static void Unsubscribe<T, THandler>()
         where T : EventBase
         where THandler : IEventHandler<T>
        {
            var name = typeof(T).Name;
            _mappings[name].Remove(typeof(THandler));

            if (_mappings[name].Count == 0)
            {
                _mappings.Remove(name);
            }
        }

        public async Task PublishAsync<T>(T o) where T : EventBase
        {
            var name = typeof(T).Name;

            if (_mappings.ContainsKey(name))
            {
                foreach (var handler in _mappings[name])
                {
                    var service = (IEventHandler<T>)_serviceProvider.GetService(handler);

                    await service.RunAsync(o);
                }
            }
        }
    }
}