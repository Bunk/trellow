using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Phone.Controls;

namespace trello.ViewModels
{
    public class ShellViewModel : ViewModelBase
    {
        private readonly BoardListViewModel _boards;
        private BoardListViewModel _boardsPivotContent;

        public BoardListViewModel BoardsPivotItem
        {
            get { return _boardsPivotContent; }
            set
            {
                _boardsPivotContent = value;
                NotifyOfPropertyChange(() => BoardsPivotItem);
            }
        }

        public ShellViewModel(BoardListViewModel boards)
        {
            BoardsPivotItem = boards;
        }

        public void PivotItemLoaded(PivotItemEventArgs e)
        {
            switch (e.Item.Name)
            {
                case "BoardsPivotItem":
                {
                    if (BoardsPivotItem == null)
                        BoardsPivotItem = _boards;
                    break;
                }
            }
        }
    }
}
