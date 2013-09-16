using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using BugSense;
using Caliburn.Micro;
using JetBrains.Annotations;
using Strilanc.Value;
using trello.Services;
using trello.Services.Messages;
using trellow.api;
using trellow.api.Boards;
using trellow.api.Cards;
using trellow.api.Lists;

namespace trello.ViewModels.Cards
{
    public class MoveCardToBoardViewModel : DialogViewModel
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly ITrello _api;
        private readonly IProgressService _progress;
        private string _originalBoardId;
        private string _originalListId;
        private Board _selectedBoard;
        private List _selectedList;
        private Action<Board, List> _accepted;

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

        public MoveCardToBoardViewModel(object root, string cardId,
                                        IEventAggregator eventAggregator,
                                        ITrello api,
                                        IProgressService progress) : base(root)
        {
            _eventAggregator = eventAggregator;
            _api = api;
            _progress = progress;

            CardId = cardId;
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

        public MoveCardToBoardViewModel OnAccepted(Action<Board, List> action)
        {
            _accepted = action;
            return this;
        }

        protected override void OnInitialize()
        {
            LoadBoards(_originalBoardId);
        }

        private async void LoadBoards(string selectedId)
        {
            _progress.Show("Loading boards...");
            try
            {
                var boards = (await _api.Boards.ForMe(BoardFilter.Open)).ToList();
                Boards.Clear();
                Boards.AddRange(boards);

                SelectedBoard = boards.FirstOrDefault(b => b.Id == selectedId);
            }
            catch (Exception ex)
            {
                BugSenseHandler.Instance.LogException(ex, new Dictionary<string, string>
                {
                    {"cardId", CardId},
                    {"boardId", _originalBoardId}
                });
                MessageBox.Show("Your boards were unable to be loaded.  Please " +
                                "ensure that you have an active internet connection.");
            }
            finally
            {
                _progress.Hide();
            }
        }

        private async void LoadListsFor(IUpdatableBoard board, May<string> selectedId)
        {
            _progress.Show("Loading lists...");
            try
            {
                var lists = (await _api.Lists.ForBoard(new BoardId(board.Id))).ToList();
                Lists.Clear();
                Lists.AddRange(lists);

                selectedId
                    .Else(lists.Take(1).Select(l => l.Id).MayFirst())
                    .IfHasValueThenDo(id =>
                    {
                        SelectedList = lists.FirstOrDefault(l => l.Id == id);
                    });
            }
            catch (Exception ex)
            {
                BugSenseHandler.Instance.LogException(ex, new Dictionary<string, string>
                {
                    {"cardId", CardId},
                    {"boardId", board.Id}
                });
                MessageBox.Show("The lists for that board were unable to be loaded.  Please " +
                                "ensure that you have an active internet connection.");
            }
            finally
            {
                _progress.Hide();
            }
        }

        public void Accept()
        {
            _eventAggregator.Publish(new CardMovedToBoard
            {
                CardId = CardId,
                BoardId = SelectedBoard.Id,
                ListId = SelectedList.Id
            });
            TryClose();

            if (_accepted != null)
                _accepted(SelectedBoard, SelectedList);
        }
    }
}