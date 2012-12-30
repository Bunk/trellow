using System;
using Caliburn.Micro;

namespace trello.ViewModels
{
    public class PivotFix<T> where T : class
    {
        private readonly Conductor<T>.Collection.OneActive _conductor;
        private bool _readyToActivate;
        private T _toActivate;
        private bool _doneReactivating;

        public PivotFix(Conductor<T>.Collection.OneActive conductor)
        {
            _conductor = conductor;
            _conductor.CloseStrategy = new DefaultCloseStrategy<T>(false);
        }

        public void OnViewLoaded(object view, Action<object> onViewLoadedBase)
        {
            onViewLoadedBase(view);

            _readyToActivate = true;
            if (_toActivate != null && !_doneReactivating)
            {
                _conductor.ActivateItem(_toActivate);
                _doneReactivating = true;
            }
        }

        public void ChangeActiveItem(T newItem, bool closePrevious, Action<T, bool> changeActiveItemBase)
        {
            if (newItem == null)
                return;

            if (!_readyToActivate && !_doneReactivating)
            {
                if (_conductor.Items.IndexOf(newItem) > 0)
                {
                    _toActivate = newItem;
                    return;
                }
            }

            changeActiveItemBase(newItem, closePrevious);
        }
    }
}