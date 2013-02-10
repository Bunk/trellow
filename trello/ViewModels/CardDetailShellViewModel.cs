using System;
using Caliburn.Micro;
using JetBrains.Annotations;
using Strilanc.Value;
using trello.Views;
using trellow.api;
using trellow.api.Data.Services;

namespace trello.ViewModels
{
    [UsedImplicitly]
    public class CardDetailShellViewModel : ViewModelBase
    {
        private readonly IWindowManager _windowManager;
        private readonly ICardService _cardService;
        private readonly Func<CardDetailViewModel> _overviewFactory;
        private CardDetailViewModel _details;

        public string Id { get; set; }

        public CardDetailViewModel Details
        {
            get { return _details; }
            set
            {
                if (Equals(value, _details)) return;
                _details = value;
                NotifyOfPropertyChange(() => Details);
            }
        }

        public CardDetailShellViewModel(ITrelloApiSettings settings,
                                        INavigationService navigation,
                                        IWindowManager windowManager,
                                        ICardService cardService,
                                        Func<CardDetailViewModel> overviewFactory) : base(settings, navigation)
        {
            _windowManager = windowManager;
            _cardService = cardService;
            _overviewFactory = overviewFactory;
        }

        protected override async void OnInitialize()
        {
            var card = await _cardService.WithId(Id);
            card.IfHasValueThenDo(x =>
            {
                Details = _overviewFactory().InitializeWith(x);
                Details.Parent = this;
            });
        }

        public void NavigateToScreen(int index)
        {
            UsingView<CardDetailShellView>(view => { view.DetailPivot.SelectedIndex = index; });
        }

        public void OpenCardName()
        {
            var model = new ChangeCardNameDialogViewModel
            {
                Name = Details.Name,
                Save = text => SaveName(text)
            };

            _windowManager.ShowDialog(model);
        }

        private void SaveName(string text)
        {
            Details.Name = text;
            // todo: Store this via the service
        }
    }

    public class ChangeCardNameDialogViewModel : Screen
    {
        private string _name;
        public Action<string> Save { get; set; }

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

        public void Accept()
        {
            Save(Name);
            TryClose();
        }
    }
}