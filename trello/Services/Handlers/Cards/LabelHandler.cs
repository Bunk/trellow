using BugSense;
using Caliburn.Micro;
using trellow.api;
using trellow.api.Cards;

namespace trello.Services.Handlers.Cards
{
    public class LabelHandler : AbstractHandler,
                                IHandle<CardLabelAdded>,
                                IHandle<CardLabelRemoved>
    {
        public LabelHandler(IEventAggregator events, ITrello api, IProgressService progress)
            : base(events, api, progress)
        {
        }

        public void Handle(CardLabelAdded message)
        {
            BugSenseHandler.Instance.SendEvent("Add label to card");
            Handle(api => api.Cards.AddLabel(new CardId(message.CardId), message.Color));
        }

        public void Handle(CardLabelRemoved message)
        {
            BugSenseHandler.Instance.SendEvent("Remove label from card");
            Handle(api => api.Cards.RemoveLabel(new CardId(message.CardId), message.Color));
        }
    }
}