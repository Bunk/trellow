using Caliburn.Micro;
using Trellow.Services.Network;
using Trellow.Services.UI;
using trellow.api;
using trellow.api.Data;
using trellow.api.Internal;

namespace Trellow.Caliburn.Micro
{
    public class ApiConfiguration
    {
        public static void Install(SimpleContainer container)
        {
#if DISCONNECTED
            container.Singleton<IRequestClient, JsonFileRestClient>();
#else
            container.Singleton<IRequestClient, TrelloRestClient>();
#endif

            var network = container.Get<INetworkService>();
            var client = AugmentClient(container);
            var trello = new Trello(network, client);
            container.Instance<ITrello>(trello);
        }

        private static IRequestClient AugmentClient(SimpleContainer container)
        {
            var progress = container.Get<IProgressService>();
            var handler = container.Get<IRequestClient>();

            return new ErrorHandlingRestClient(new ProgressAwareRestClient(handler, progress));
        }
    }
}