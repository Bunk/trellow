using Caliburn.Micro;
using Trellow.Diagnostics;
using Trellow.Events;
using Trellow.Services.UI;
using trellow.api;
using trellow.api.Cards;

namespace Trellow.Handlers.Cards
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