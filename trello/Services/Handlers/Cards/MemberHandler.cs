using BugSense;
using Caliburn.Micro;
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
            BugSenseHandler.Instance.SendEvent("Add member to card");
            Handle(api => api.Cards.AddMember(new CardId(message.CardId), new MemberId(message.MemberId)));
        }

        public void Handle(CardMemberRemoved message)
        {
            BugSenseHandler.Instance.SendEvent("Remove member from card");
            Handle(api => api.Cards.RemoveMember(new CardId(message.CardId), new MemberId(message.MemberId)));
        }
    }
}