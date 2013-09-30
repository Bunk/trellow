using System.Linq;
using Caliburn.Micro;
using Trellow.Handlers;

namespace Trellow.Caliburn.Micro
{
    public static class EventHandlers
    {
        public static void Install(SimpleContainer container)
        {
            var assembly = typeof (AbstractHandler).Assembly;
            container.AllSingletonTypesOf<AbstractHandler>(assembly);

            // this forces instantiation
            container.GetAllInstances(typeof (AbstractHandler)).ToList();
        }
    }
}