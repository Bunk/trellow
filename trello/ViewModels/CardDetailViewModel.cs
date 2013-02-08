using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using Caliburn.Micro;
using JetBrains.Annotations;
using Microsoft.Phone.Controls;
using trello.ViewModels.Activities;
using trellow.api;
using trellow.api.Data.Services;
using trellow.api.Models;

namespace trello.ViewModels
{
    [UsedImplicitly]
    public class CardDetailViewModel : ViewModelBase
    {
        private readonly ICardService _cardService;
        private readonly Func<ChecklistViewModel> _checkListFactory;
        private readonly Func<AttachmentViewModel> _attachmentFactory;
        private bool _editingDesc;
        private string _name;
        private string _desc;
        private string _originalDesc;
        private int _votes;

        public CardDetailViewModel(ITrelloApiSettings settings,
                                   INavigationService navigation,
                                   ICardService cardService,
                                   Func<ChecklistViewModel> checkListFactory,
                                   Func<AttachmentViewModel> attachmentFactory) : base(settings, navigation)
        {
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

        public string Id { get; set; }

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

        public DateTime? Due { get; set; }

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

        public CardDetailViewModel InitializeWith(Card card)
        {
            Id = card.Id;
            Name = card.Name;
            Desc = card.Desc;
            Votes = card.Badges.Votes;

            if (card.Badges.Due.HasValue)
            {
                var utc = DateTime.SpecifyKind(card.Badges.Due.Value, DateTimeKind.Utc);
                Due = utc.ToLocalTime();
            }

            Labels.Clear();
            Labels.AddRange(card.Labels.Select(x => new LabelViewModel(x)));

            Checklists.Clear();
            Checklists.AddRange(card.Checklists.Select(x =>
            {
                var checklist = _checkListFactory().For(x, card.CheckItemStates);
                checklist.PropertyChanged += (sender, args) =>
                {
                    NotifyOfPropertyChange(() => CheckItems);
                    NotifyOfPropertyChange(() => CheckItemsChecked);
                };
                return checklist;
            }));

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

        protected override void OnInitialize()
        {
        }

        public void GoToChecklists()
        {
            NavigateToScreen(1);
        }

        public void GoToAttachments()
        {
            NavigateToScreen(2);
        }

        public void GoToMembers()
        {
            NavigateToScreen(3);
        }

        public void GoToActivity()
        {
            NavigateToScreen(4);
        }

        private void NavigateToScreen(int index)
        {
            var parent = Parent as CardDetailShellViewModel;
            if (parent == null)
                return;

            parent.NavigateToScreen(index);
        }

        public void EditDesc(GestureEventArgs args)
        {
            OriginalDesc = ((TextBlock) args.OriginalSource).Text;
            EditingDesc = true;
        }

        public async Task UpdateDesc()
        {
            // todo: Probably want to institute the command pattern and queue them up in case we're offline
            await _cardService.UpdateDescription(Id, Desc);

            EditingDesc = false;
        }

        public void CancelDesc()
        {
            Desc = OriginalDesc;
            EditingDesc = false;
        }

        public void RemoveAttachment()
        {
        }

        public void AddAttachment()
        {
        }
    }
}