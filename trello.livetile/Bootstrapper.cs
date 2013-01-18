using Caliburn.Micro;
using trellow.api;
using trellow.api.Data;
using trellow.api.OAuth;

namespace trello.livetile
{
    public static class Bootstrapper
    {
        public static SimpleContainer Container { get; private set; }

        static Bootstrapper()
        {
            Container = new SimpleContainer();

            Container.Singleton<IRequestProcessor, RequestProcessor>();
            Container.Singleton<ITrelloApiSettings, TrelloSettings>();

            RegisterRepository(Container);
        }

        private static void RegisterRepository(SimpleContainer container)
        {
            container.Singleton<IOAuthClient, TrelloOAuthClient>();

            container.Singleton<IBoardService, BoardService>();
            container.Singleton<ICardService, CardService>();
            container.Singleton<IProfileService, ProfileService>();
        }
    }
}
