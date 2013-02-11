using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using JetBrains.Annotations;
using trello.Services.Handlers;
using trellow.api;
using trellow.api.Models;

namespace trello.ViewModels
{
    [UsedImplicitly]
    public class ChecklistViewModel : ViewModelBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly Func<ChecklistItemViewModel> _itemFactory;
        private string _name;

        private string Id { get; set; }

        private string CardId { get; set; }

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

        public IObservableCollection<ChecklistItemViewModel> Items { get; set; }
        // ReSharper restore MemberCanBePrivate.Global

        public int ItemsChecked
        {
            get { return Items.Count(i => i.Checked); }
        }

        public ChecklistViewModel(ITrelloApiSettings settings,
                                  INavigationService navigation,
                                  IEventAggregator eventAggregator,
                                  Func<ChecklistItemViewModel> itemFactory) : base(settings, navigation)
        {
            _eventAggregator = eventAggregator;
            _itemFactory = itemFactory;

            Items = new BindableCollection<ChecklistItemViewModel>();
            Items.CollectionChanged += (sender, args) => NotifyOfPropertyChange(() => ItemsChecked);
        }

        private readonly List<PropertyObserver<ChecklistItemViewModel>> _itemsObservers =
            new List<PropertyObserver<ChecklistItemViewModel>>();

        public ChecklistViewModel For(CheckList checkList, Card card)
        {
            Id = checkList.Id;
            Name = checkList.Name;
            CardId = card.Id;

            Items.Clear();
            _itemsObservers.ForEach(o => o.UnregisterHandler(i => i.Checked));
            _itemsObservers.Clear();

            Items.AddRange(checkList.CheckItems.Select(x =>
            {
                var item = _itemFactory().For(x);

                // note: This is the way that the Trello API seems to work currently--hopefully they fix it
                if (card.CheckItemStates.Any(
                    c => c.IdCheckItem == item.Id &&
                         c.State == CheckListItem.CheckState.Complete))
                {
                    item.Checked = true;
                }

                // wire up to make sure we update the server when this changes.
                _itemsObservers.Add(new PropertyObserver<ChecklistItemViewModel>(item)
                                        .RegisterHandler(i => i.Checked, CheckItemChanged));

                return item;
            }));

            return this;
        }

        private void CheckItemChanged(ChecklistItemViewModel item)
        {
            NotifyOfPropertyChange(() => ItemsChecked);

            _eventAggregator.Publish(new CheckItemChanged
            {
                CardId = CardId,
                ChecklistId = Id,
                CheckItemId = item.Id,
                Value = item.Checked
            });
        }
    }
}