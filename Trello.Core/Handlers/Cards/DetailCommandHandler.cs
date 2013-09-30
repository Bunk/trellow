using Caliburn.Micro;
using Trellow.Diagnostics;
using Trellow.Events;
using Trellow.Services.UI;
using trellow.api;
using trellow.api.Cards;
using trellow.api.Lists;

namespace Trellow.Handlers.Cards
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
            Analytics.TagEvent("Update_Card_Description");
            Handle(api => api.Cards.ChangeDescription(new CardId(message.CardId), message.Description));
        }

        public void Handle(CardDueDateChanged message)
        {
            Analytics.TagEvent("Update_Card_Due");
            Handle(api => api.Cards.ChangeDueDate(new CardId(message.CardId), message.DueDate));
        }

        public void Handle(CardNameChanged message)
        {
            Analytics.TagEvent("Update_Card_Name");
            Handle(api => api.Cards.ChangeName(new CardId(message.CardId), message.Name));
        }

        public void Handle(CardCommented message)
        {
            Analytics.TagEvent("Comment_Card");
            Handle(api => api.Cards.AddComment(new CardId(message.CardId), message.Text));
        }

        public void Handle(CardCreationRequested message)
        {
            Analytics.TagEvent("Create_Card");
            Handle(async api =>
            {
                var created = await api.Cards.Add(new NewCard(message.Name, new ListId(message.ListId)));
                Analytics.TagEvent("Created_Card");

                Events.Publish(new CardCreated {Card = created});
            });
        }
    }
}