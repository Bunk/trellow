using System;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using JetBrains.Annotations;
using trello.Interactions;
using trellow.api.Cards;

namespace trello.ViewModels
{
    public class CardViewModel : Screen
    {
        private readonly INavigationService _navigationService;
        private InteractionManager _interactionManager;
        private string _name;
        private string _boardName;
        private string _listName;
        private string _desc;
        private DateTime? _due;
        private int _votes;
        private int _comments;
        private int _checkItems;
        private int _checkItemsChecked;
        private int _attachments;
        private string _coverUri;
        private int _coverHeight;
        private int _coverWidth;
        private double _pos;

        public string Id { get; set; }

        public string BoardId { get; set; }

        public string ListId { get; set; }

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

        public double Pos
        {
            get { return _pos; }
            set
            {
                if (value.Equals(_pos)) return;
                _pos = value;
                NotifyOfPropertyChange(() => Pos);
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

        public int Comments
        {
            get { return _comments; }
            set
            {
                if (value == _comments) return;
                _comments = value;
                NotifyOfPropertyChange(() => Comments);
            }
        }

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

        public IObservableCollection<MemberViewModel> Members { get; set; }

        public IObservableCollection<LabelViewModel> Labels { get; set; }

        public Card OriginalCard { get; set; }

        public CardViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            Members = new BindableCollection<MemberViewModel>();
            Labels = new BindableCollection<LabelViewModel>();
        }

        public CardViewModel InitializeWith(Card card)
        {
            OriginalCard = card;

            var cover = card.Attachments.SingleOrDefault(att => att.Id == card.IdAttachmentCover);

            BoardName = card.Board != null ? card.Board.Name : null;
            ListName = card.List != null ? card.List.Name : null;

            Id = card.Id;
            BoardId = card.IdBoard;
            ListId = card.IdList;
            Name = card.Name;
            Desc = card.Desc;
            Due = card.Due;
            Votes = card.Badges.Votes;
            Comments = card.Badges.Comments;
            CheckItems = card.Badges.CheckItems;
            CheckItemsChecked = card.Badges.CheckItemsChecked;
            Attachments = card.Badges.Attachments;
            Pos = card.Pos;

            CoverUri = cover != null ? cover.Previews.First().Url : null;
            CoverHeight = cover != null ? cover.Previews.First().Height : 0;
            CoverWidth = cover != null ? cover.Previews.First().Width : 0;

            Members.Clear();
            Members.AddRange(card.Members.Select(mem => new MemberViewModel(mem)));

            Labels.Clear();
            Labels.AddRange(card.Labels.Select(lbl => new LabelViewModel(lbl.Color.ToString(), lbl.Name)));

            return this;
        }

        public CardViewModel EnableInteractions(InteractionManager interactionManager)
        {
            // We need to defer to the OnViewLoaded event in order to reference
            // an actual view.  At this point it hasn't been created, yet.
            _interactionManager = interactionManager;
            return this;
        }

        public CardViewModel DisableInteractions()
        {
            var view = (FrameworkElement) GetView();
            _interactionManager.RemoveElement(view);
            return this;
        }

        protected override void OnViewLoaded(object view)
        {
            // the first time the view is loaded, we want to add
            // this element to the interaction manager
            var element = view as FrameworkElement;
            if (element != null && _interactionManager != null)
                _interactionManager.AddElement(element);
        }

        [UsedImplicitly]
        public void Open()
        {
            _navigationService.UriFor<CardDetailPivotViewModel>()
                              .WithParam(x => x.Id, Id)
                              .Navigate();
        }
    }
}