using System.Linq;
using Caliburn.Micro;
using JetBrains.Annotations;
using Microsoft.Phone.Shell;
using TrelloNet;
using trello.Assets;
using trello.Services;
using trello.Services.Handlers;

namespace trello.ViewModels
{
    public sealed class CardDetailMembersViewModel : PivotItemViewModel,
                                                     IConfigureTheAppBar,
                                                     IHandle<CardMemberAdded>,
                                                     IHandle<CardMemberRemoved>
    {
        private readonly ITrello _api;
        private readonly IProgressService _progress;
        private readonly IEventAggregator _eventAggregator;
        private readonly IWindowManager _windowManager;

        public string Id { get; private set; }

        public string BoardId { get; private set; }

        public IObservableCollection<MemberViewModel> Members { get; set; }

        public CardDetailMembersViewModel(ITrello api,
                                          IProgressService progress,
                                          IEventAggregator eventAggregator,
                                          IWindowManager windowManager)
        {
            DisplayName = "members";

            _api = api;
            _progress = progress;
            _eventAggregator = eventAggregator;
            _windowManager = windowManager;

            _eventAggregator.Subscribe(this);

            Members = new BindableCollection<MemberViewModel>();
        }

        public CardDetailMembersViewModel Initialize(Card card)
        {
            Id = card.Id;
            BoardId = card.IdBoard;

            var mems = card.Members.Select(mem => new MemberViewModel(mem));
            Members.Clear();
            Members.AddRange(mems);

            return this;
        }

        [UsedImplicitly]
        public void AssignMembers()
        {
            var members = Members.Select(m => m.Id).ToList();
            var model = new ChangeCardMembersViewModel(GetView(),
                                                       _eventAggregator, _api, _progress,
                                                       Id, BoardId, members);
            _windowManager.ShowDialog(model);
        }

        public void Handle(CardMemberAdded message)
        {
            Members.Add(new MemberViewModel(message.Member));

            _eventAggregator.Publish(new MemberAggregationsUpdated {AssignedMemberCount = Members.Count});
        }

        public void Handle(CardMemberRemoved message)
        {
            var found = Members.FirstOrDefault(m => m.Id == message.Member.Id);
            if (found != null)
                Members.Remove(found);

            _eventAggregator.Publish(new MemberAggregationsUpdated {AssignedMemberCount = Members.Count});
        }

        public ApplicationBar Configure(ApplicationBar existing)
        {
            var button = new ApplicationBarIconButton(new AssetUri("Icons/dark/appbar.feature.settings.rest.png")) { Text = "assign" };
            button.Click += (sender, args) => AssignMembers();
            existing.Buttons.Add(button);

            return existing;
        }

        public class MemberAggregationsUpdated
        {
            public int AssignedMemberCount { get; set; }
        }
    }
}