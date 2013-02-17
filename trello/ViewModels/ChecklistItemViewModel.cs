using Caliburn.Micro;
using JetBrains.Annotations;
using trello.Services.Handlers;
using trellow.api.Models;
using Card = TrelloNet.Card;

namespace trello.ViewModels
{
    [UsedImplicitly]
    public class ChecklistItemViewModel : PropertyChangedBase
    {
        private readonly IEventAggregator _eventAggregator;

        private string _id;
        private string _idCard;
        private string _idList;
        private string _name;
        private bool _checked;

        public string Id
        {
            get { return _id; }
            set
            {
                if (value == _id) return;
                _id = value;
                NotifyOfPropertyChange(() => Id);
            }
        }

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

        public bool Checked
        {
            get { return _checked; }
            set
            {
                if (value.Equals(_checked)) return;
                _checked = value;

                NotifyOfPropertyChange(() => Checked);
            }
        }

        public ChecklistItemViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public void Toggle()
        {
            Checked = !Checked;

            _eventAggregator.Publish(new CheckItemChanged
            {
                CardId = _idCard,
                ChecklistId = _idList,
                CheckItemId = Id,
                Value = Checked
            });
        }

        public ChecklistItemViewModel InitializeWith(string cardId, string listId, Card.CheckItem item)
        {
            _idCard = cardId;
            _idList = listId;

            Id = item.Id;
            Name = item.Name;
            Checked = item.Checked;

            return this;
        }
    }
}