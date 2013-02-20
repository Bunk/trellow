using System;
using Caliburn.Micro;

namespace trello
{
    public class SuppressNotificationScope : IDisposable
    {
        private readonly bool _wasNotifying;
        private PropertyChangedBase _model;

        public SuppressNotificationScope(PropertyChangedBase model)
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
