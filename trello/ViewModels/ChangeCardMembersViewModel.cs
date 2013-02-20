using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using JetBrains.Annotations;
using TrelloNet;
using trello.Services;
using trello.Services.Handlers;

namespace trello.ViewModels
{
    public class ChangeCardMembersViewModel : DialogViewModel
    {
        private readonly IEventAggregator _aggregator;
        private readonly ITrello _api;
        private readonly IProgressService _progressService;
        private readonly string _cardId;
        private readonly string _boardId;
        private readonly IList<string> _existingIds;

        public IObservableCollection<SelectedMemberViewModel> Members { get; set; }

        public ChangeCardMembersViewModel(object root,
                                          IEventAggregator eventAggregator,
                                          ITrello api,
                                          IProgressService progressService,
                                          string cardId,
                                          string boardId,
                                          IList<string> existingIds)
            : base(root)
        {
            _aggregator = eventAggregator;
            _api = api;
            _progressService = progressService;
            _cardId = cardId;
            _boardId = boardId;
            _existingIds = existingIds;

            Members = new BindableCollection<SelectedMemberViewModel>();
        }

        protected override async void OnInitialize()
        {
            _progressService.Show("Loading members...");
            try
            {
                var all = await _api.Async.Members.ForBoard(new BoardId(_boardId));
                var tfd = all.Select(CreateModel);

                Members.Clear();
                Members.AddRange(tfd);
            }
            catch (Exception)
            {
                MessageBox.Show("The board's members could not be loaded.  Please " +
                                "ensure that you have an active internet connection.");
            }
            finally
            {
                _progressService.Hide();
            }
        }

        [UsedImplicitly]
        public void Toggle(SelectedMemberViewModel model)
        {
            if (model.Toggle())
                _aggregator.Publish(new CardMemberAdded {CardId = _cardId, Member = model.Member});
            else
                _aggregator.Publish(new CardMemberRemoved {CardId = _cardId, Member = model.Member});
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