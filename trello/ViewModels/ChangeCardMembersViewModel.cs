using Caliburn.Micro;
using System.Collections.Generic;
using System.Linq;
using TrelloNet;
using trello.Services.Handlers;

namespace trello.ViewModels
{
    public class ChangeCardMembersViewModel : DialogViewModel
    {
        private readonly IEventAggregator _aggregator;
        private readonly ITrello _api;
        private readonly IList<string> _existingIds;

        public string CardId { get; set; }

        public string BoardId { get; set; }

        public IObservableCollection<SelectedMemberViewModel> Members { get; set; }

        public ChangeCardMembersViewModel(object root, IEventAggregator eventAggregator, ITrello api, IList<string> existingIds)
            : base(root)
        {
            _aggregator = eventAggregator;
            _api = api;
            _existingIds = existingIds;

            Members = new BindableCollection<SelectedMemberViewModel>();
        }

        protected override async void OnInitialize()
        {
            var all = await _api.Async.Members.ForBoard(new BoardId(BoardId));
            var tfd = all.Select(CreateModel);
            
            Members.Clear();
            Members.AddRange(tfd);
        }

        public void Toggle(SelectedMemberViewModel model)
        {
            if (model.Toggle())
                _aggregator.Publish(new CardMemberAdded { CardId = CardId, Member = model.Member });
            else
                _aggregator.Publish(new CardMemberRemoved { CardId = CardId, Member = model.Member });
        }

        private SelectedMemberViewModel CreateModel(Member member)
        {
            var selected = _existingIds.Any(id => id == member.Id);
            var vm = new SelectedMemberViewModel(member, selected);
            return vm;
        }

        public class SelectedMemberViewModel : PropertyChangedBase
        {
            private bool _assigned;

            public Member Member { get; set; }

            public string ImageUriLarge { get; set; } 

            public bool Assigned
            {
                get { return _assigned; }
                set
                {
                    if (value.Equals(_assigned)) return;
                    _assigned = value;
                    NotifyOfPropertyChange(() => Assigned);
                }
            }

            public SelectedMemberViewModel(Member member, bool selected)
            {
                Member = member;
                Assigned = selected;

                ImageUriLarge = string.Format("https://trello-avatars.s3.amazonaws.com/{0}/170.png", member.AvatarHash);
            }

            public bool Toggle()
            {
                return (Assigned = !Assigned);
            }
        }
    }
}
