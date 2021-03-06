﻿using System;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using JetBrains.Annotations;
using trello.Assets;
using trello.Extensions;
using trello.Services;
using trello.Services.Messages;
using trello.ViewModels.Activities;
using trellow.api;
using trellow.api.Actions;
using trellow.api.Cards;
using trellow.api.Members;
using Action = System.Action;

namespace trello.ViewModels.Cards
{
    [UsedImplicitly]
    public sealed class CardDetailOverviewViewModel : PivotItemViewModel<CardDetailOverviewViewModel>,
                                                      IHandle<CardDescriptionChanged>,
                                                      IHandle<CardDueDateChanged>,
                                                      IHandle<CardLabelAdded>,
                                                      IHandle<CardLabelRemoved>,
                                                      IHandle<CardMovedToBoard>,
                                                      IHandle<CardDetailChecklistViewModel.AggregationsUpdated>,
                                                      IHandle<CardDetailMembersViewModel.MemberAggregationsUpdated>
    {
        private readonly ITrello _api;
        private readonly ITrelloApiSettings _settings;
        private readonly IEventAggregator _eventAggregator;
        private readonly INavigationService _navigation;
        private readonly IProgressService _progress;
        private readonly IWindowManager _windowManager;
        private string _boardId;
        private string _listId;
        private int _attachments;
        private int _checkItems;
        private int _checkItemsChecked;
        private int _checklists;
        private string _desc;
        private DateTime? _due;
        private int _members;
        private string _name;
        private int _votes;
        private string _myAvatarUrl;
        private string _coverUri;
        private int _coverHeight;
        private int _coverWidth;
        private string _commentText;
        private string _boardName;
        private string _listName;

        [UsedImplicitly]
        public string Id { get; private set; }

        [UsedImplicitly]
        public string Name
        {
            get { return _name; }
            set
            {
                if (value == _name) return;
                _name = value;

                NotifyOfPropertyChange(() => Name);
            }
        }

        [UsedImplicitly]
        public string BoardName
        {
            get { return _boardName; }
            set
            {
                if (value == _boardName) return;
                _boardName = value;
                NotifyOfPropertyChange(() => BoardName);
            }
        }

        [UsedImplicitly]
        public string ListName
        {
            get { return _listName; }
            set
            {
                if (value == _listName) return;
                _listName = value;
                NotifyOfPropertyChange(() => ListName);
            }
        }

        [UsedImplicitly]
        public string Desc
        {
            get { return _desc; }
            set
            {
                if (value == _desc) return;
                _desc = value;
                NotifyOfPropertyChange(() => Desc);
            }
        }

        [UsedImplicitly]
        public DateTime? Due
        {
            get { return _due; }
            set
            {
                if (value.Equals(_due)) return;
                _due = value;
                NotifyOfPropertyChange(() => Due);
            }
        }

        [UsedImplicitly]
        public int Checklists
        {
            get { return _checklists; }
            set
            {
                if (value == _checklists) return;
                _checklists = value;
                NotifyOfPropertyChange(() => Checklists);
            }
        }

        [UsedImplicitly]
        public int CheckItems
        {
            get { return _checkItems; }
            set
            {
                if (value == _checkItems) return;
                _checkItems = value;
                NotifyOfPropertyChange(() => CheckItems);
            }
        }

        [UsedImplicitly]
        public int CheckItemsChecked
        {
            get { return _checkItemsChecked; }
            set
            {
                if (value == _checkItemsChecked) return;
                _checkItemsChecked = value;
                NotifyOfPropertyChange(() => CheckItemsChecked);
            }
        }

        [UsedImplicitly]
        public int Votes
        {
            get { return _votes; }
            set
            {
                if (value == _votes) return;
                _votes = value;
                NotifyOfPropertyChange(() => Votes);
            }
        }

        [UsedImplicitly]
        public int Attachments
        {
            get { return _attachments; }
            set
            {
                if (value == _attachments) return;
                _attachments = value;
                NotifyOfPropertyChange(() => Attachments);
            }
        }

        [UsedImplicitly]
        public int Members
        {
            get { return _members; }
            set
            {
                if (value == _members) return;
                _members = value;
                NotifyOfPropertyChange(() => Members);
            }
        }

        [UsedImplicitly]
        public string CoverUri
        {
            get { return _coverUri; }
            set
            {
                if (value == _coverUri) return;
                _coverUri = value;
                NotifyOfPropertyChange(() => CoverUri);
            }
        }

        [UsedImplicitly]
        public int CoverHeight
        {
            get { return _coverHeight; }
            set
            {
                if (value == _coverHeight) return;
                _coverHeight = value;
                NotifyOfPropertyChange(() => CoverHeight);
            }
        }

        [UsedImplicitly]
        public int CoverWidth
        {
            get { return _coverWidth; }
            set
            {
                if (value == _coverWidth) return;
                _coverWidth = value;
                NotifyOfPropertyChange(() => CoverWidth);
            }
        }

        [UsedImplicitly]
        public string MyAvatarUrl
        {
            get { return _myAvatarUrl; }
            set
            {
                if (value == _myAvatarUrl) return;
                _myAvatarUrl = value;
                NotifyOfPropertyChange(() => MyAvatarUrl);
            }
        }

        [UsedImplicitly]
        public string CommentText
        {
            get { return _commentText; }
            set
            {
                if (value == _commentText) return;
                _commentText = value;
                NotifyOfPropertyChange(() => CommentText);
            }
        }

        [UsedImplicitly]
        public IObservableCollection<LabelViewModel> Labels { get; set; }

        [UsedImplicitly]
        public IObservableCollection<ActivityViewModel> Comments { get; set; }

        public CardDetailOverviewViewModel(ITrello api,
                                           ITrelloApiSettings settings,
                                           IProgressService progress,
                                           IEventAggregator eventAggregator,
                                           INavigationService navigation,
                                           IWindowManager windowManager)
        {
            DisplayName = "overview";

            _api = api;
            _settings = settings;
            _progress = progress;
            _navigation = navigation;
            _windowManager = windowManager;
            _eventAggregator = eventAggregator;

            // we need to listen to other events even when not active
            _eventAggregator.Subscribe(this);

            Labels = new BindableCollection<LabelViewModel>();
            Comments = new BindableCollection<ActivityViewModel>();
        }

        public CardDetailOverviewViewModel Initialize(Card card)
        {
            _boardId = card.IdBoard;
            _listId = card.IdList;

            Id = card.Id;
            Name = card.Name;
            BoardName = card.Board.Name;
            ListName = card.List.Name;
            Desc = card.Desc;
            Due = card.Due;
            Checklists = card.Checklists.Count;
            CheckItems = card.Checklists.Aggregate(0, (i, model) => i + model.CheckItems.Count);
            CheckItemsChecked = card.Checklists.Aggregate(0,
                                                          (i, model) => i + model.CheckItems.Count(item => item.Checked));
            Votes = card.Badges.Votes;
            Attachments = card.Attachments.Count;
            Members = card.Members.Count;

            var cover = card.Attachments.SingleOrDefault(att => att.Id == card.IdAttachmentCover);
            CoverUri = cover != null ? cover.Previews.First().Url : null;
            CoverHeight = cover != null ? cover.Previews.First().Height : 0;
            CoverWidth = cover != null ? cover.Previews.First().Width : 0;

            var lbls = card.Labels.Select(lbl => new LabelViewModel(lbl.Color.ToString(), lbl.Name));
            Labels.Clear();
            Labels.AddRange(lbls);

            MyAvatarUrl = _settings.AvatarHash.ToAvatarUrl(AvatarSize.Portrait);

            return this;
        }

        protected override async void OnInitialize()
        {
            var types = new[] {ActionType.CommentCard};
            var actions = await _api.Actions.ForCard(new CardId(Id), types, paging: new Paging(25, 0));

            var vms = actions.Select(ActivityViewModel.InitializeWith).ToList();

            Comments.Clear();
            Comments.AddRange(vms);
        }

        protected override void OnActivate()
        {
            ApplicationBar.UpdateWith(builder =>
            {
                builder.Setup(bar =>
                {
                    bar.AddButton("archive card", new AssetUri("Icons/dark/appbar.delete.rest.png"), ArchiveCard);

                    bar.AddMenuItem("set due", ChangeDueDate);
                    bar.AddMenuItem("change labels", ChangeLabels);
                    bar.AddMenuItem("move card", MoveToBoard);
                    bar.AddMenuItem("delete card", DeleteCard);
                });
                builder.Defaults();
            });
        }

        private void EnsureId(string id, Action action)
        {
            if (Id == id)
                action();
        }

        [UsedImplicitly]
        public void GoToChecklists()
        {
            _eventAggregator.Publish(new CardDetailPivotViewModel.PivotRequestingNavigation
            {
                Screen = CardDetailPivotViewModel.Screen.Checklists
            });
        }

        [UsedImplicitly]
        public void GoToAttachments()
        {
            _eventAggregator.Publish(new CardDetailPivotViewModel.PivotRequestingNavigation
            {
                Screen = CardDetailPivotViewModel.Screen.Attachments
            });
        }

        [UsedImplicitly]
        public void GoToMembers()
        {
            _eventAggregator.Publish(new CardDetailPivotViewModel.PivotRequestingNavigation
            {
                Screen = CardDetailPivotViewModel.Screen.Members
            });
        }

        [UsedImplicitly]
        public void ChangeDueDate()
        {
            var model = new ChangeCardDueViewModel(GetView())
            {
                CardId = Id,
                Date = Due
            };
            _windowManager.ShowDialog(model.Bind(ApplicationBar));
        }

        [UsedImplicitly]
        public void ChangeDescription()
        {
            var model = new ChangeCardDescriptionViewModel(GetView())
            {
                CardId = Id,
                Description = Desc
            };

            _windowManager.ShowDialog(model.Bind(ApplicationBar));
        }

        [UsedImplicitly]
        public void ChangeLabels()
        {
            var selected = Labels.Select(lbl => (Color) Enum.Parse(typeof (Color), lbl.Color));

            var model = new ChangeCardLabelsViewModel(GetView()) {CardId = Id}
                .Initialize(selected)
                .Bind(ApplicationBar);

            _windowManager.ShowDialog(model);
        }

        [UsedImplicitly]
        public void Comment(string comment)
        {
            if (string.IsNullOrWhiteSpace(comment))
                return;

            CommentText = null;

            _eventAggregator.Publish(new CardCommented
            {
                CardId = Id,
                MemberId = _settings.MemberId,
                Text = comment
            });

            Comments.Insert(0, new CommentActivityViewModel
            {
                Member = new MemberViewModel(new Member
                {
                    AvatarHash = _settings.AvatarHash,
                    FullName = _settings.Fullname,
                    Username = _settings.Username
                }),
                Text = comment,
                Timestamp = DateTime.Now
            });
        }

        [UsedImplicitly]
        public void ArchiveCard()
        {
            var result = MessageBox.Show(
                "This card will be archived.  Do you really want to archive it?",
                "confirm archive", MessageBoxButton.OKCancel);
            if (result != MessageBoxResult.OK) return;

            _eventAggregator.Publish(new CardArchived {CardId = Id});
            _navigation.GoBack();
        }

        [UsedImplicitly]
        public void DeleteCard()
        {
            var result = MessageBox.Show(
                "This card will be removed forever.  This is a permanent action that cannot be undone.\n\n" +
                "Do you really want to delete it?",
                "confirm delete", MessageBoxButton.OKCancel);
            if (result != MessageBoxResult.OK) return;

            _eventAggregator.Publish(new CardDeleted {CardId = Id});
            _navigation.GoBack();
        }

        [UsedImplicitly]
        public void MoveToBoard()
        {
            var model = new MoveCardToBoardViewModel(GetView()) {CardId = Id}
                .Initialize(_boardId, _listId)
                .Bind(ApplicationBar);

            _windowManager.ShowDialog(model);
        }

        public void Handle(CardMovedToBoard message)
        {
            EnsureId(message.CardId, () =>
            {
                _boardId = message.BoardId;
                BoardName = message.BoardName;

                _listId = message.ListId;
                ListName = message.ListName;
            });
        }

        public void Handle(CardDescriptionChanged message)
        {
            EnsureId(message.CardId, () => Desc = message.Description);
        }

        public void Handle(CardDueDateChanged message)
        {
            EnsureId(message.CardId, () => Due = message.DueDate);
        }

        public void Handle(CardLabelAdded message)
        {
            EnsureId(message.CardId, () => Labels.Add(new LabelViewModel(message.Color.ToString(), message.Name)));
        }

        public void Handle(CardLabelRemoved message)
        {
            EnsureId(message.CardId, () =>
            {
                var found = Labels.FirstOrDefault(lbl => lbl.Color == message.Color.ToString());
                if (found != null)
                    Labels.Remove(found);
            });
        }

        public void Handle(CardDetailChecklistViewModel.AggregationsUpdated message)
        {
            EnsureId(message.CardId, () =>
            {
                Checklists = message.ChecklistCount;

                // note: ProgressBar control has some quirks  (value must always be <= maximum)
                // http://geekswithblogs.net/cskardon/archive/2011/02/06/silverlight-progressbar-issues-with-binding.aspx
                CheckItemsChecked = 0;
                CheckItems = message.CheckItemsCount;
                CheckItemsChecked = message.CheckItemsCheckedCount;
            });
        }

        public void Handle(CardDetailMembersViewModel.MemberAggregationsUpdated message)
        {
            EnsureId(message.CardId, () => Members = message.AssignedMemberCount);
        }
    }
}