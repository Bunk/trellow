using System;
using System.Linq;
using System.Windows.Controls;
using Caliburn.Micro;
using Microsoft.Phone.Controls;
using trellow.api.Models;

namespace trello.ViewModels
{
    public class CardDetailOverviewViewModel : PivotItemViewModel
    {
        private bool _editingDesc;
        private string _name;
        private string _desc;
        private string _originalDesc;
        private int _checkItems;
        private int _checkItemsChecked;
        private int _checklists;
        private int _members;
        private int _attachments;
        private int _votes;

        public string ViewContext { get; set; }

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

        public CardDetailOverviewViewModel()
        {
            DisplayName = "overview";
        }

        public CardDetailOverviewViewModel InitializeWith(Card card)
        {
            Id = card.Id;
            Name = card.Name;
            Desc = card.Desc;

            Checklists = card.IdChecklists.Count;
            CheckItems = card.Badges.CheckItems;
            CheckItemsChecked = card.Badges.CheckItemsChecked;
            Members = card.Members.Count;
            Attachments = card.Badges.Attachments;
            Votes = card.Badges.Votes;

            if (card.Badges.Due.HasValue)
            {
                var utc = DateTime.SpecifyKind(card.Badges.Due.Value, DateTimeKind.Utc);
                Due = utc.ToLocalTime();
            }

            Labels = new BindableCollection<LabelViewModel>(card.Labels.Select(x => new LabelViewModel(x)));

            return this;
        }

        protected override void OnInitialize()
        {
        }

        public void EditDesc(GestureEventArgs args)
        {
            OriginalDesc = ((TextBlock) args.OriginalSource).Text;
            EditingDesc = true;
        }

        public void UpdateDesc()
        {
            // todo: Async update Trello
            // todo: Probably want to institute the command pattern and queue them up in case we're offline
            EditingDesc = false;
        }

        public void CancelDesc()
        {
            Desc = OriginalDesc;
            EditingDesc = false;
        }
    }
}