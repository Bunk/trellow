using System;
using System.ComponentModel;
using System.Windows;
using Caliburn.Micro;
using JetBrains.Annotations;
using trello.Services;

namespace trello.ViewModels
{
    public interface INeedApplicationBar<out T> where T : class
    {
        /// <summary>
        /// Binds the given application bar to this screen instance.
        /// Any modifications to this object will show up on all other bound
        /// screens.
        /// </summary>
        /// <param name="applicationBar">The application bar to bind</param>
        T Bind(IApplicationBar applicationBar);
    }

    public abstract class PivotItemViewModel<T> : Screen, INeedApplicationBar<T> where T : class
    {
        protected IApplicationBar ApplicationBar;

        /// <summary>
        /// Binds the given application bar to this screen instance.
        /// Any modifications to this object will show up on all other bound
        /// screens.
        /// </summary>
        /// <param name="applicationBar">The application bar to bind</param>
        public T Bind(IApplicationBar applicationBar)
        {
            ApplicationBar = applicationBar;
            return this as T;
        }
    }

    public abstract class PivotViewModel : Conductor<IScreen>.Collection.OneActive
    {
        protected readonly INavigationService Navigation;
        private readonly PivotFix<IScreen> _pivotFix;

        [UsedImplicitly]
        public IApplicationBar AppBar { get; private set; }

        protected PivotViewModel(INavigationService navigation, IApplicationBar applicationBar)
        {
            Navigation = navigation;
            AppBar = applicationBar;

            _pivotFix = new PivotFix<IScreen>(this);
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

        protected override void OnViewLoaded(object view)
        {
            _pivotFix.OnViewLoaded(view, base.OnViewLoaded);
        }

        protected override void ChangeActiveItem(IScreen newItem, bool closePrevious)
        {
            if (AppBar.Instance != null)
                AppBar.Instance.IsVisible = false;

            _pivotFix.ChangeActiveItem(newItem, closePrevious, base.ChangeActiveItem);
        }

        protected void UsingView<T>(Action<T> action) where T : class
        {
            var view = base.GetView() as T;
            if (view == null)
            {
                MessageBox.Show("The view could not be found.");
                return;
            }
            action(view);
        }

        private void ApplicationBarUpdated(object sender, PropertyChangedEventArgs args)
        {
            NotifyOfPropertyChange(() => AppBar.Instance);
        }
    }
}