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
    }
}
