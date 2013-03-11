using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Caliburn.Micro;
using JetBrains.Annotations;
using TrelloNet;
using trello.Extensions;
using trello.Services;
using trello.Services.Handlers;

namespace trello.ViewModels
{
    public sealed class CardDetailMembersViewModel : PivotItemViewModel
    {
        private readonly ITrello _api;
        private readonly IProgressService _progress;
        private readonly IEventAggregator _eventAggregator;
        private readonly IObservableCollection<CardMemberViewModel> _members;
        private readonly IObservableCollection<CardMemberViewModel> _otherMembers;
        private int _allMembersCount;

        public string Id { get; private set; }

        public string BoardId { get; private set; }

        public int AllMembersCount
        {
            get { return _allMembersCount; }
            set
            {
                if (value == _allMembersCount) return;
                _allMembersCount = value;
                NotifyOfPropertyChange(() => AllMembersCount);
            }
        }

        public CollectionViewSource Members { get; set; }

        public CollectionViewSource OtherMembers { get; set; }

        public CardDetailMembersViewModel(ITrello api,
                                          IProgressService progress,
                                          IEventAggregator eventAggregator)
        {
            DisplayName = "members";

            _api = api;
            _progress = progress;
            _eventAggregator = eventAggregator;

            _eventAggregator.Subscribe(this);

            AllMembersCount = 0;
            _members = new BindableCollection<CardMemberViewModel>();
            _otherMembers = new BindableCollection<CardMemberViewModel>();

            Members = new CollectionViewSource();
            Members.SortDescriptions.Add(new SortDescription("FullName", ListSortDirection.Descending));
            Members.Source = _members;

            OtherMembers = new CollectionViewSource();
            OtherMembers.SortDescriptions.Add(new SortDescription("FullName", ListSortDirection.Descending));
            OtherMembers.Source = _otherMembers;
        }

        protected override async void OnInitialize()
        {
            try
            {
                _progress.Show("Loading board members...");

                var boardMembers = await _api.Members.ForBoard(new BoardId(BoardId));
                var others = boardMembers.Where(mem => _members.All(m => m.Id != mem.Id));
                var otherVms = others.Select(mem => new CardMemberViewModel(mem, false));

                _otherMembers.Clear();
                _otherMembers.AddRange(otherVms);
                AllMembersCount = _members.Count + _otherMembers.Count;
            }
            catch (TrelloException)
            {
                MessageBox.Show("Could not load the members for this board.  Please " +
                                "ensure that you have an active internet connection.");
            }
            finally
            {
                _progress.Hide();
            }
        }

        public CardDetailMembersViewModel Initialize(Card card)
        {
            Id = card.Id;
            BoardId = card.IdBoard;

            var mems = card.Members.Select(mem => new CardMemberViewModel(mem, true));
            _members.Clear();
            _members.AddRange(mems);

            return this;
        }

        [UsedImplicitly]
        public void Toggle(CardMemberViewModel model)
        {
            if (model.Toggle())
            {
                // now attached
                _otherMembers.Remove(model);
                _members.Add(model);

                _eventAggregator.Publish(new CardMemberAdded {CardId = Id, MemberId = model.Id});
            }
            else
            {
                // now unattached
                _members.Remove(model);
                _otherMembers.Insert(0, model);

                _eventAggregator.Publish(new CardMemberRemoved {CardId = Id, MemberId = model.Id});
            }
            _eventAggregator.Publish(new MemberAggregationsUpdated {AssignedMemberCount = _members.Count});
        }

        public class MemberAggregationsUpdated
        {
            public int AssignedMemberCount { get; set; }
        }

        public class CardMemberViewModel : PropertyChangedBase
        {
            private bool _attached;

            public string Id { get; set; }

            public string FullName { get; set; }

            public string Username { get; set; }

            public string AvatarUrl { get; set; }

            public bool Attached
            {
                get { return _attached; }
                set
                {
                    if (value.Equals(_attached)) return;
                    _attached = value;
                    NotifyOfPropertyChange(() => Attached);
                }
            }

            public CardMemberViewModel(Member member, bool attached)
            {
                Id = member.Id;
                FullName = member.FullName;
                Username = member.Username;
                Attached = attached;
                AvatarUrl = member.AvatarHash.ToAvatarUrl(AvatarSize.Portrait);
            }

            public bool Toggle()
            {
                return (Attached = !Attached);
            }
        }
    }
}