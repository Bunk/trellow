using System;
using System.Linq;
using System.Reflection;
using Caliburn.Micro;

namespace trello.Extensions
{
    public static class SimpleContainerExtensions
    {
        public static T Get<T>(this SimpleContainer container)
        {
            return (T) container.GetInstance(typeof (T), null);
        }

        public static T Get<T>(this SimpleContainer container, string key)
        {
            return (T) container.GetInstance(typeof (T), key);
        }

        public static SimpleContainer AllTransientTypesOf<TService>(this SimpleContainer container,
                                                                    Assembly assembly = null,
                                                                    Func<Type, bool> filter = null)
        {
            if (assembly == null)
                assembly = Assembly.GetExecutingAssembly();

            if (filter == null)
                filter = type => true;

            var serviceType = typeof (TService);
            var matching = assembly.GetTypes().Where(type =>
            {
                if (serviceType.IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface)
                    return filter(type);

                return false;
            });
            foreach (var implementation in matching)
            {
                container.RegisterPerRequest(typeof (TService), null, implementation);
                container.RegisterPerRequest(implementation, null, implementation);
            }
            return container;
        }
    }
}