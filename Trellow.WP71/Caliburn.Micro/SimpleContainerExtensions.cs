using System;
using System.Linq;
using System.Reflection;
using Caliburn.Micro;

namespace Trellow.Caliburn.Micro
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
            return AllTypesOf<TService>(container, type =>
            {
                container.RegisterPerRequest(typeof (TService), null, type);
                container.RegisterPerRequest(type, null, type);
            }, filter, assembly, false);
        }

        public static SimpleContainer AllSingletonTypesOf<TService>(this SimpleContainer container,
                                                                    Assembly assembly = null,
                                                                    Func<Type, bool> filter = null,
                                                                    bool instantiateImmediately = false)
        {
            return AllTypesOf<TService>(container, type =>
            {
                container.RegisterSingleton(type, null, type);
                container.RegisterHandler(typeof(TService), null, c => c.GetInstance(type, null));
                if (instantiateImmediately)
                    container.GetInstance(type, null);
            }, filter, assembly, instantiateImmediately);
        }

        public static SimpleContainer AllTypesOf<TService>(this SimpleContainer container,
                                                           Action<Type> register,
                                                           Func<Type, bool> filter = null,
                                                           Assembly assembly = null,
                                                           bool instantiateImmediately = false)
        {
            var abstractType = typeof (TService);
            var augmentedFilter = new Func<Type, bool>(type =>
            {
                if (abstractType.IsAssignableFrom(type))
                {
                    return filter == null || filter(type);
                }
                return false;
            });
            return AllTypesWhere(container, augmentedFilter, register, assembly);
        }

        public static SimpleContainer AllTypesWhere(this SimpleContainer container,
                                                    Func<Type, bool> filter,
                                                    Action<Type> register,
                                                    Assembly assembly = null,
                                                    bool instantiateImmediately = false)
        {
            if (assembly == null)
                assembly = Assembly.GetExecutingAssembly();

            if (filter == null)
                filter = type => true;

            var matching = assembly.GetTypes().Where(type =>
            {
                if (!type.IsAbstract && !type.IsInterface)
                    return filter(type);

                return false;
            });
            foreach (var implementation in matching)
            {
                register(implementation);
            }
            return container;
        }

        public static bool InNamespace(this Type type, string name)
        {
            return type.Namespace != null && type.Namespace.StartsWith(name);
        }
    }
}