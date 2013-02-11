using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;
using JetBrains.Annotations;
using Microsoft.Phone.Controls;
using Strilanc.Value;
using trello.Services.Handlers;
using trello.ViewModels.Activities;
using trello.Views;
using trellow.api;
using trellow.api.Data.Services;
using trellow.api.Models;

namespace trello.ViewModels
{
    [UsedImplicitly]
    public class CardDetailViewModel : ViewModelBase
    {
        private readonly IWindowManager _windowManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly ICardService _cardService;
        private readonly Func<ChecklistViewModel> _checkListFactory;
        private readonly Func<AttachmentViewModel> _attachmentFactory;
        private bool _editingDesc;
        private string _name;
        private string _desc;
        private string _originalDesc;
        private int _votes;
        private DateTime? _due;
        private LabelNames _labelNames;

        public CardDetailViewModel(ITrelloApiSettings settings,
                                   INavigationService navigation,
                                   IWindowManager windowManager,
                                   IEventAggregator eventAggregator,
                                   ICardService cardService,
                                   Func<ChecklistViewModel> checkListFactory,
                                   Func<AttachmentViewModel> attachmentFactory) : base(settings, navigation)
        {
            _windowManager = windowManager;
            _eventAggregator = eventAggregator;
            _cardService = cardService;
            _checkListFactory = checkListFactory;
            _attachmentFactory = attachmentFactory;
            Labels = new BindableCollection<LabelViewModel>();
            Checklists = new BindableCollection<ChecklistViewModel>();
            Attachments = new BindableCollection<AttachmentViewModel>();
            Members = new BindableCollection<MemberViewModel>();
            Activities = new BindableCollection<ActivityViewModel>();
            Comments = new BindableCollection<ActivityViewModel>();
        }

        public string Id { get; private set; }

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

        private CardDetailViewModel InitializeWith(Card card)
        {
            Id = card.Id;
            Name = card.Name;
            Desc = card.Desc;
            Due = card.Due;
            Votes = card.Badges.Votes;

            _labelNames = card.Board.LabelNames;

            Labels.Clear();
            Labels.AddRange(card.Labels.Select(x => new LabelViewModel(x)));

            Checklists.Clear();
            Checklists.AddRange(card.Checklists.Select(x =>
            {
                var checklist = _checkListFactory().For(x, card);
                checklist.PropertyChanged += (sender, args) =>
                {
                    NotifyOfPropertyChange(() => CheckItems);
                    NotifyOfPropertyChange(() => CheckItemsChecked);
                };
                return checklist;
            }));
            NotifyOfPropertyChange(() => CheckItems);
            NotifyOfPropertyChange(() => CheckItemsChecked);

            Attachments.Clear();
            Attachments.AddRange(card.Attachments.Select(x => _attachmentFactory().For(x)));

            Members.Clear();
            Members.AddRange(card.Members.Select(x => new MemberViewModel(x)));

            Activities.Clear();
            Activities.AddRange(card.Actions.Select(ActivityViewModel.For));

            Comments.Clear();
            Comments.AddRange(card.Actions.Where(c => c.Type == ActivityType.CommentCard).Select(ActivityViewModel.For));

            if (!string.IsNullOrWhiteSpace(card.IdAttachmentCover))
            {
                var coverAttachment = Attachments.FirstOrDefault(x => x.Id == card.IdAttachmentCover);
                if (coverAttachment != null)
                    coverAttachment.IsCover = true;
            }

            return this;
        }

        protected override async void OnInitialize()
        {
            var card = await _cardService.WithId(Id);
            card.IfHasValueThenDo(x => InitializeWith(x));
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
        public void ChangeLabels()
        {
            var model = new ChangeCardLabelsViewModel(GetView(), _labelNames, Labels)
            {
                Accepted = x =>
                {
                    Labels.Clear();
                    Labels.AddRange(x.Select(l => new LabelViewModel(l)));

                    _eventAggregator.Publish(new CardLabelsChanged {CardId = Id, Labels = x});
                }
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
    }
}