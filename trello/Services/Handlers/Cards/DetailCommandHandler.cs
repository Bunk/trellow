using BugSense;
using Caliburn.Micro;
using trellow.api;
using trellow.api.Cards;
using trellow.api.Lists;

namespace trello.Services.Handlers.Cards
{
    public class DetailCommandHandler : AbstractHandler,
                                        IHandle<CardNameChanged>,
                                        IHandle<CardDescriptionChanged>,
                                        IHandle<CardDueDateChanged>,
                                        IHandle<CardCommented>,
                                        IHandle<CardCreationRequested>
    {
        public DetailCommandHandler(IEventAggregator events, ITrello api, IProgressService progress)
            : base(events, api, progress)
        {
        }

        public void Handle(CardDescriptionChanged message)
        {
            BugSenseHandler.Instance.SendEvent("Update card description");
            Handle(api => api.Cards.ChangeDescription(new CardId(message.CardId), message.Description));
        }

        public void Handle(CardDueDateChanged message)
        {
            BugSenseHandler.Instance.SendEvent("Update card due date");
            Handle(api => api.Cards.ChangeDueDate(new CardId(message.CardId), message.DueDate));
        }

        public void Handle(CardNameChanged message)
        {
            BugSenseHandler.Instance.SendEvent("Change card name");
            Handle(api => api.Cards.ChangeName(new CardId(message.CardId), message.Name));
        }

        public void Handle(CardCommented message)
        {
            BugSenseHandler.Instance.SendEvent("Comment on card");
            Handle(api => api.Cards.AddComment(new CardId(message.CardId), message.Text));
        }

        public void Handle(CardCreationRequested message)
        {
            BugSenseHandler.Instance.SendEvent("Create card");
            Handle(async api =>
            {
                var created = await api.Cards.Add(new NewCard(message.Name, new ListId(message.ListId)));
                Events.Publish(new CardCreated {Card = created});
            });
        }
    }
}