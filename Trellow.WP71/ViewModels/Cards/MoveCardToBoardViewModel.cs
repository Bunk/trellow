using System.ComponentModel;
using System.Linq;
using Caliburn.Micro;
using JetBrains.Annotations;
using Strilanc.Value;
using Trellow.Events;
using Trellow.UI;
using trellow.api;
using trellow.api.Boards;
using trellow.api.Lists;

namespace Trellow.ViewModels.Cards
{
    public class MoveCardToBoardViewModel : DialogViewModel
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly ITrello _api;
        private string _originalBoardId;
        private string _originalListId;
        private Board _selectedBoard;
        private List _selectedList;

        [UsedImplicitly]
        public string CardId { get; set; }

        [UsedImplicitly]
        public Board SelectedBoard
        {
            get { return _selectedBoard; }
            set
            {
                if (value == _selectedBoard) return;
                _selectedBoard = value;
                NotifyOfPropertyChange(() => SelectedBoard);
            }
        }

        [UsedImplicitly]
        public List SelectedList
        {
            get { return _selectedList; }
            set
            {
                if (Equals(value, _selectedList)) return;
                _selectedList = value;
                NotifyOfPropertyChange(() => SelectedList);
            }
        }

        [UsedImplicitly]
        public IObservableCollection<Board> Boards { get; set; }

        [UsedImplicitly]
        public IObservableCollection<List> Lists { get; set; }

        public MoveCardToBoardViewModel(object root) : base(root)
        {
            _eventAggregator = IoC.Get<IEventAggregator>();
            _api = IoC.Get<ITrello>();

            Boards = new BindableCollection<Board>();
            Lists = new BindableCollection<List>();

            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == "SelectedBoard")
            {
                var listId = SelectedBoard.Id == _originalBoardId
                                 ? _originalListId
                                 : May<string>.NoValue;
                LoadListsFor(SelectedBoard, listId);
            }
        }

        public MoveCardToBoardViewModel Initialize(string boardId, string listId)
        {
            _originalBoardId = boardId;
            _originalListId = listId;

            return this;
        }

        protected override void OnInitialize()
        {
            LoadBoards(_originalBoardId);
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            UpdateApplicationBar(bar =>
            {
                bar.AddButton("accept", new AssetUri("Icons/dark/appbar.check.rest.png"), Accept);
                bar.AddButton("cancel", new AssetUri("Icons/dark/appbar.close.rest.png"), TryClose);
            });
        }

        private async void LoadBoards(string selectedId)
        {
            var boards = (await _api.Boards.ForMe(BoardFilter.Open)).ToList();
            Boards.Clear();
            Boards.AddRange(boards);

            SelectedBoard = boards.FirstOrDefault(b => b.Id == selectedId);
        }

        private async void LoadListsFor(IUpdatableBoard board, May<string> selectedId)
        {
            var lists = (await _api.Lists.ForBoard(new BoardId(board.Id))).ToList();
            Lists.Clear();
            Lists.AddRange(lists);

            selectedId
                .Else(lists.Take(1).Select(l => l.Id).MayFirst())
                .IfHasValueThenDo(id => { SelectedList = lists.FirstOrDefault(l => l.Id == id); });
        }

        private void Accept()
        {
            _eventAggregator.Publish(new CardMovedToBoard
            {
                CardId = CardId,
                BoardId = SelectedBoard.Id,
                BoardName = SelectedBoard.Name,
                ListId = SelectedList.Id,
                ListName = SelectedList.Name
            });
            TryClose();
        }
    }
}