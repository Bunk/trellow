using Caliburn.Micro;
using trello.Services.Messages;
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
            Analytics.TagEvent("Update_Card_Label_Add");
            Handle(api => api.Cards.AddLabel(new CardId(message.CardId), message.Color));
        }

        public void Handle(CardLabelRemoved message)
        {
            Analytics.TagEvent("Update_Card_Label_Remove");
            Handle(api => api.Cards.RemoveLabel(new CardId(message.CardId), message.Color));
        }
    }
}