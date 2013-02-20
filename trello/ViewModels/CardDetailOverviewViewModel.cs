using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using JetBrains.Annotations;
using Microsoft.Phone.Shell;
using TrelloNet;
using trello.Services;
using trello.Services.Handlers;
using trello.Views;
using trellow.api;

namespace trello.ViewModels
{
    public class CardDetailPivotViewModel : PivotViewModel,
        IHandle<CardDetailPivotViewModel.PivotRequestingNavigation>
    {
        public enum Screen
        {
            Overview, Checklists, Attachments, Members
        }

        private readonly ITrello _api;
        private readonly IEventAggregator _eventAggregator;
        private readonly IProgressService _progress;
        private readonly Func<CardDetailOverviewViewModel> _overview;
        private readonly Func<CardDetailChecklistViewModel> _checklists;
        private string _name;

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

        public CardDetailPivotViewModel(
            ITrello api,
            ITrelloApiSettings settings, 
            INavigationService navigation,
            IEventAggregator eventAggregator,
            IProgressService progress,
            Func<CardDetailOverviewViewModel> overview,
            Func<CardDetailChecklistViewModel> checklists) : base(settings, navigation)
        {
            _api = api;
            _progress = progress;
            _overview = overview;
            _checklists = checklists;

            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
        }

        protected override async void OnInitialize()
        {
            _progress.Show("Loading...");

            var card = new Card();
            try
            {
                card = await _api.Async.Cards.WithId(Id);
            }
            catch
            {
                MessageBox.Show("Could not load this card.  Please ensure that you " +
                                "have an active internet connection.");
            }

            Name = card.Name;

            Items.Add(_overview().Initialize(card));
            Items.Add(_checklists().Initialize(card));

            ActivateItem(Items[0]);

            _progress.Hide();
        }

        public class PivotRequestingNavigation
        {
            public Screen Screen { get; set; }
        }

        public void Handle(PivotRequestingNavigation message)
        {
            var index = GetScreenIndex(message.Screen);
            UsingView<CardDetailPivotView>(view => view.Items.SelectedIndex = index);
        }

        private static int GetScreenIndex(Screen screen)
        {
            switch (screen)
            {
                case Screen.Checklists:
                    return 1;
                case Screen.Attachments:
                    return 2;
                case Screen.Members:
                    return 3;
                default:
                    return 0;
            }
        }
    }

    public sealed class CardDetailOverviewViewModel : PivotItemViewModel, 
        IConfigureTheAppBar,
        IHandle<CardDetailChecklistViewModel.ChecklistAggregationsUpdated>
    {
        private readonly ITrello _api;
        private readonly IProgressService _progress;
        private readonly IEventAggregator _eventAggregator;
        private readonly IWindowManager _windowManager;
        private string _name;
        private string _desc;
        private int _votes;
        private DateTime? _due;
        private int _checkItems;
        private int _checkItemsChecked;
        private int _attachments;
        private int _checklists;
        private int _members;
        private Dictionary<Color, string> _labelNames;

        public string Id { get; private set; }

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

        public string CoverUri { get; set; }

        public int CoverHeight { get; set; }

        public int CoverWidth { get; set; }

        public IObservableCollection<LabelViewModel> Labels { get; set; }

        public CardDetailOverviewViewModel(ITrello api, IProgressService progress, IEventAggregator eventAggregator, IWindowManager windowManager)
        {
            DisplayName = "overview";

            _api = api;
            _progress = progress;
            _eventAggregator = eventAggregator;
            _windowManager = windowManager;
            _eventAggregator.Subscribe(this);

            Labels = new BindableCollection<LabelViewModel>();
        }

        public CardDetailOverviewViewModel Initialize(Card card)
        {
            Id = card.Id;
            Name = card.Name;
            Desc = card.Desc;
            Due = card.Due;
            Checklists = card.Checklists.Count;
            CheckItems = card.Checklists.Aggregate(0, (i, model) => i + model.CheckItems.Count);
            CheckItemsChecked = card.Checklists.Aggregate(0, (i, model) => i + model.CheckItems.Count(item => item.Checked));
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

            return this;
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
        public async void ChangeLabels()
        {
            if (_labelNames == null)
            {
                _progress.Show("Loading label names...");
                try
                {
                    var board = await _api.Async.Boards.ForCard(new CardId(Id));
                    _labelNames = board.LabelNames;
                }
                catch (Exception)
                {
                    MessageBox.Show("The label names were unable to be loaded.  Please " +
                                    "ensure that you have an active internet connection.");
                    return;
                }
                finally
                {
                    _progress.Hide();
                }
            }

            var model = new ChangeCardLabelsViewModel(GetView(), _labelNames, Labels)
            {
                Accepted = newLabels =>
                {
                    var oldLabels = Labels.Select(l => new Card.Label
                    {
                        Color = (Color)Enum.Parse(typeof(Color), l.Color),
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

        public void Handle(CardDetailChecklistViewModel.ChecklistAggregationsUpdated message)
        {
            Checklists = message.ChecklistCount;
            CheckItems = message.CheckItemsCount;
            CheckItemsChecked = message.CheckItemsCheckedCount;
        }

        public ApplicationBar ConfigureTheAppBar(ApplicationBar existing)
        {
            existing.Buttons.Add(new AppBarButton
            {
                Text = "delete card",
                IconUri = new Uri("/Assets/Icons/dark/appbar.delete.rest.png", UriKind.Relative),
                Message = "DeleteCard"
            });
            return existing;
        }

        public void DeleteCard()
        {
            MessageBox.Show("We should delete the card now.");
        }
    }

    public sealed class CardDetailChecklistViewModel : PivotItemViewModel, IHandle<CheckItemChanged>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly Func<ChecklistViewModel> _checklistFactory;

        public IObservableCollection<ChecklistViewModel> Checklists { get; set; }

        public CardDetailChecklistViewModel(IEventAggregator eventAggregator, Func<ChecklistViewModel> checklistFactory)
        {
            DisplayName = "checklists";

            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
            _checklistFactory = checklistFactory;

            Checklists = new BindableCollection<ChecklistViewModel>();
        }

        public CardDetailChecklistViewModel Initialize(Card card)
        {
            var checks = card.Checklists.Select(list => _checklistFactory().InitializeWith(list));
            Checklists.Clear();
            Checklists.AddRange(checks);

            return this;
        }

        public void Handle(CheckItemChanged message)
        {
            var update = new ChecklistAggregationsUpdated
            {
                ChecklistCount = Checklists.Count,
                CheckItemsCount = Checklists.Aggregate(0, (i, model) => i + model.Items.Count),
                CheckItemsCheckedCount = Checklists.Aggregate(0, (i, model) => i + model.ItemsChecked)
            };
            _eventAggregator.Publish(update);
        }

        public class ChecklistAggregationsUpdated
        {
            public int ChecklistCount { get; set; }

            public int CheckItemsCount { get; set; }

            public int CheckItemsCheckedCount { get; set; }
        }
    }
}
