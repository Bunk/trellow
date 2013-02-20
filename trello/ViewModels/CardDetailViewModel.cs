using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;
using JetBrains.Annotations;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using TrelloNet;
using trello.Services;
using trello.Services.Handlers;
using trello.ViewModels.Activities;
using trello.Views;
using trellow.api;
using Action = TrelloNet.Action;

namespace trello.ViewModels
{
    [UsedImplicitly]
    public class CardDetailViewModel : ViewModelBase,
        IHandle<CheckItemChanged>,
        IHandle<CardMemberAdded>,
        IHandle<CardMemberRemoved>
    {
        public IApplicationBar ApplicationBar { get; set; }
        private readonly IWindowManager _windowManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly ITrello _api;
        private readonly IProgressService _progress;
        private readonly Func<ChecklistViewModel> _checkListFactory;
        private readonly Func<AttachmentViewModel> _attachmentFactory;
        private bool _editingDesc;
        private string _name;
        private string _desc;
        private string _originalDesc;
        private int _votes;
        private DateTime? _due;
        private Dictionary<Color, string> _labelNames;

        public CardDetailViewModel(ITrelloApiSettings settings,
                                   ITrello api,
                                   IProgressService progress,
                                   INavigationService navigation,
                                   IWindowManager windowManager,
                                   IEventAggregator eventAggregator,
                                   Func<ChecklistViewModel> checkListFactory,
                                   Func<AttachmentViewModel> attachmentFactory) : base(settings, navigation)
        {
            _windowManager = windowManager;
            _eventAggregator = eventAggregator;
            _api = api;
            _progress = progress;
            _checkListFactory = checkListFactory;
            _attachmentFactory = attachmentFactory;

            _eventAggregator.Subscribe(this);

            Labels = new BindableCollection<LabelViewModel>();
            Checklists = new BindableCollection<ChecklistViewModel>();
            Attachments = new BindableCollection<AttachmentViewModel>();
            Members = new BindableCollection<MemberViewModel>();
            Activities = new BindableCollection<ActivityViewModel>();
            Comments = new BindableCollection<ActivityViewModel>();
        }

        public string Id { get; private set; }

        public string BoardId { get; private set; }

// ReSharper disable MemberCanBePrivate.Global
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

        public string OriginalDesc
        {
            get { return _originalDesc; }
            set
            {
                if (value == _originalDesc) return;
                _originalDesc = value;
                NotifyOfPropertyChange(() => OriginalDesc);
            }
        }

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

        public int CheckItems
        {
            get { return Checklists.Aggregate(0, (i, model) => i + model.Items.Count); }
        }

        public int CheckItemsChecked
        {
            get { return Checklists.Aggregate(0, (i, model) => i + model.ItemsChecked); }
        }

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

        public IObservableCollection<LabelViewModel> Labels { get; set; }

        public IObservableCollection<ChecklistViewModel> Checklists { get; set; }

        public IObservableCollection<AttachmentViewModel> Attachments { get; set; }

        public IObservableCollection<MemberViewModel> Members { get; set; }

        public IObservableCollection<ActivityViewModel> Activities { get; set; }

        public IObservableCollection<ActivityViewModel> Comments { get; set; }

        public bool EditingDesc
        {
            get { return _editingDesc; }
            set
            {
                if (value.Equals(_editingDesc)) return;
                _editingDesc = value;
                NotifyOfPropertyChange(() => EditingDesc);
            }
        }

// ReSharper restore MemberCanBePrivate.Global

        private void InitializeCard(Card card)
        {
            Id = card.Id;
            BoardId = card.IdBoard;
            Name = card.Name;
            Desc = card.Desc;
            Due = card.Due;
            Votes = card.Badges.Votes;

            var lbls = card.Labels.Select(lbl => new LabelViewModel(lbl.Color.ToString(), lbl.Name));
            Labels.Clear();
            Labels.AddRange(lbls);

            var checks = card.Checklists.Select(list => _checkListFactory().InitializeWith(list));
            Checklists.Clear();
            Checklists.AddRange(checks);
            NotifyCheckItemsChanged();

            var atts = card.Attachments.Select(att => _attachmentFactory().InitializeWith(att));
            Attachments.Clear();
            Attachments.AddRange(atts);

            var mems = card.Members.Select(mem => new MemberViewModel(mem));
            Members.Clear();
            Members.AddRange(mems);

            if (string.IsNullOrWhiteSpace(card.IdAttachmentCover))
                return;

            var coveratt = Attachments.FirstOrDefault(x => x.Id == card.IdAttachmentCover);
            if (coveratt != null)
                coveratt.IsCover = true;
        }

        private void InitializeActivity(IEnumerable<Action> actions)
        {
            var vms = actions.Select(ActivityViewModel.InitializeWith).ToList();
            Activities.Clear();
            Activities.AddRange(vms);

            Comments.Clear();
            Comments.AddRange(vms);
        }

        protected override async void OnInitialize()
        {
            _progress.Show("Loading...");

            try
            {
                var details = await _api.Async.Cards.WithId(Id);
                InitializeCard(details);

                var activity = await _api.Async.Actions.ForCard(details, new[] {ActionType.CommentCard}, paging: new Paging(25, 0));
                InitializeActivity(activity);
            }
            catch (Exception)
            {
                MessageBox.Show("Could not load this card.  Please ensure that you " +
                                "have an active internet connection.");
            }

            _progress.Hide();
        }

        [UsedImplicitly]
        public void GoToChecklists()
        {
            NavigateToScreen(1);
        }

        [UsedImplicitly]
        public void GoToAttachments()
        {
            NavigateToScreen(2);
        }

        [UsedImplicitly]
        public void GoToMembers()
        {
            NavigateToScreen(3);
        }

        [UsedImplicitly]
        public void GoToActivity()
        {
            NavigateToScreen(4);
        }

        private void NavigateToScreen(int index)
        {
            UsingView<CardDetailView>(view => { view.DetailPivot.SelectedIndex = index; });
        }

        [UsedImplicitly]
        public void ChangeName()
        {
            var model = new ChangeCardNameViewModel(GetView())
            {
                Name = Name,
                Accepted = text =>
                {
                    Name = text;
                    _eventAggregator.Publish(new CardNameChanged {CardId = Id, Name = Name});
                }
            };

            _windowManager.ShowDialog(model);
        }

        [UsedImplicitly]
        public void ChangeDueDate()
        {
            var model = new ChangeCardDueViewModel(GetView())
            {
                Date = Due,
                Accepted = d =>
                {
                    Due = d;
                    _eventAggregator.Publish(new CardDueDateChanged {CardId = Id, DueDate = d});
                },
                Removed = () =>
                {
                    Due = null;
                    _eventAggregator.Publish(new CardDueDateChanged {CardId = Id, DueDate = null});
                }
            };

            _windowManager.ShowDialog(model);
        }

        [UsedImplicitly]
        public async void ChangeLabels()
        {
            if (_labelNames == null)
            {
                var board = await _api.Async.Boards.ForCard(new CardId(Id));
                _labelNames = board.LabelNames;
            }

            var model = new ChangeCardLabelsViewModel(GetView(), _labelNames, Labels)
            {
                Accepted = newLabels =>
                {
                    var oldLabels = Labels.Select(l => new Card.Label
                    {
                        Color = (Color) Enum.Parse(typeof (Color), l.Color),
                        Name = l.Name
                    }).ToList();

                    _eventAggregator.Publish(new CardLabelsChanged
                    {
                        CardId = Id,
                        LabelsAdded = newLabels.Except(oldLabels, l => l.Color).ToList(),
                        LabelsRemoved = oldLabels.Except(newLabels, l => l.Color).ToList()
                    });

                    Labels.Clear();
                    Labels.AddRange(newLabels.Select(l => new LabelViewModel(l.Color.ToString(), l.Name)));
                }
            };
            _windowManager.ShowDialog(model);
        }

        [UsedImplicitly]
        public void ChangeMembers()
        {
            var members = Members.Select(m => m.Id).ToList();
            var model = new ChangeCardMembersViewModel(GetView(), _eventAggregator, _api, members)
            {
                BoardId = BoardId,
                CardId = Id
            };
            _windowManager.ShowDialog(model);
        }

        public void EditDesc()
        {
            OriginalDesc = Desc;
            EditingDesc = true;
        }

        public void UpdateDesc()
        {
            _eventAggregator.Publish(new CardDescriptionChanged {CardId = Id, Description = Desc});

            EditingDesc = false;
        }

        public void CancelDesc()
        {
            Desc = OriginalDesc;
            EditingDesc = false;
        }

        private void NotifyCheckItemsChanged()
        {
            NotifyOfPropertyChange(() => CheckItems);
            NotifyOfPropertyChange(() => CheckItemsChecked);
        }

        public void Handle(CheckItemChanged message)
        {
            NotifyCheckItemsChanged();
        }

        public void Handle(CardMemberAdded message)
        {
            Members.Add(new MemberViewModel(message.Member));
        }

        public void Handle(CardMemberRemoved message)
        {
            var found = Members.FirstOrDefault(m => m.Id == message.Member.Id);
            if (found != null)
                Members.Remove(found);
        }
    }
}