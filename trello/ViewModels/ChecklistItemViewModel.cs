using System.Windows;
using Caliburn.Micro;
using JetBrains.Annotations;
using trello.Services.Handlers;
using trello.Services.Messages;
using trellow.api.Cards;
using trellow.api.Checklists;

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

        [UsedImplicitly]
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

            // note: Should not leak--handling a reference to myself
            PropertyChanged += (sender, args) =>
            {
                if (!IsNotifying) 
                    return;

                if (args.PropertyName == "Checked")
                {
                    _eventAggregator.Publish(new CheckItemChanged
                    {
                        CardId = _idCard,
                        ChecklistId = _idList,
                        CheckItemId = Id,
                        Value = Checked
                    });
                }
            };
        }

        [UsedImplicitly]
        public void Toggle()
        {
            Checked = !Checked;
        }

        [UsedImplicitly]
        public void Remove()
        {
            _eventAggregator.Publish(new CheckItemRemoved { ChecklistId = _idList, CheckItemId = Id });
        }

        /// <summary>
        /// This is used to initialize a check list item that is already attached to a pre-existing card.
        /// </summary>
        public ChecklistItemViewModel InitializeWith(string cardId, string listId, Card.CheckItem item)
        {
            using (new SuppressNotificationScope(this))
            {
                _idCard = cardId;
                _idList = listId;

                Id = item.Id;
                Name = item.Name;
                Checked = item.Checked;
            }

            return this;
        }

        /// <summary>
        /// This is used to initialize a check list item that has been created and returned from the API
        /// </summary>
        public ChecklistItemViewModel InitializeWith(string cardId, string listId, CheckItem item)
        {
            using (new SuppressNotificationScope(this))
            {
                _idCard = cardId;
                _idList = listId;

                Id = item.Id;
                Name = item.Name;
                Checked = false;
            }
            return this;
        }
    }
}