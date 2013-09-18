using Caliburn.Micro;
using trello.Services.Messages;
using trellow.api;
using trellow.api.Cards;
using trellow.api.Members;

namespace trello.Services.Handlers.Cards
{
    public class MemberHandler : AbstractHandler,
                                 IHandle<CardMemberAdded>,
                                 IHandle<CardMemberRemoved>
    {
        public MemberHandler(IEventAggregator events, ITrello api, IProgressService progress)
            : base(events, api, progress)
        {
        }

        public void Handle(CardMemberAdded message)
        {
            Analytics.TagEvent("Update_Card_Member_Add");
            Handle(api => api.Cards.AddMember(new CardId(message.CardId), new MemberId(message.MemberId)));
        }

        public void Handle(CardMemberRemoved message)
        {
            Analytics.TagEvent("Update_Card_Member_Remove");
            Handle(api => api.Cards.RemoveMember(new CardId(message.CardId), new MemberId(message.MemberId)));
        }
    }
}