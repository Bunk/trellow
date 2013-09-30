using System.ComponentModel;
using Caliburn.Micro;
using Trellow.Services.UI;
using JetBrains.Annotations;

namespace Trellow.ViewModels
{
    public abstract class ViewModelBase : Screen
    {
        [UsedImplicitly]
        public IApplicationBar AppBar { get; private set; }

        protected ViewModelBase(IApplicationBar applicationBar)
        {
            AppBar = applicationBar;
        }

        protected override void OnActivate()
        {
            AppBar.PropertyChanged += ApplicationBarUpdated;
            base.OnActivate();
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            AppBar.PropertyChanged -= ApplicationBarUpdated;
        }

        private void ApplicationBarUpdated(object sender, PropertyChangedEventArgs args)
        {
            NotifyOfPropertyChange(() => AppBar.Instance);
        }
    }
}