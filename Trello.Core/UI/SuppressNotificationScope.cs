using System;
using Caliburn.Micro;

namespace Trellow.UI
{
    public class SuppressNotificationScope : IDisposable
    {
        private readonly bool _wasNotifying;
        private INotifyPropertyChangedEx _model;

        public SuppressNotificationScope(INotifyPropertyChangedEx model)
        {
            _wasNotifying = model.IsNotifying;

            _model = model;
            _model.IsNotifying = false;
        }

        public void Dispose()
        {
            _model.IsNotifying = _wasNotifying;
            _model = null;
        }
    }
}
