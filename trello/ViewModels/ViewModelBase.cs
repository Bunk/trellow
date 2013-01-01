using System;
using System.Collections.Generic;
using System.Windows;
using Caliburn.Micro;
using Microsoft.Phone.Shell;

namespace trello.ViewModels
{
    public abstract class ViewModelBase : Screen
    {
        protected ApplicationBar _defaultAppBar;

        public ApplicationBar DefaultAppBar
        {
            get { return _defaultAppBar; }
            set
            {
                _defaultAppBar = value;
                NotifyOfPropertyChange(() => DefaultAppBar);
            }
        }

        public ViewModelBase()
        {
            SetUpDefaultAppBar();
        }

        private void SetUpDefaultAppBar()
        {
            var bar = new ApplicationBar {IsVisible = true, IsMenuEnabled = true, Opacity = 1};

            var accountSettings = new ApplicationBarMenuItem("account");
            bar.MenuItems.Add(accountSettings);

            var appSettings = new ApplicationBarMenuItem("settings");
            bar.MenuItems.Add(appSettings);

            DefaultAppBar = bar;
        }

        protected override void OnViewReady(object view)
        {
            if (!(view is DependencyObject))
                throw new InvalidOperationException("The view must be a DependencyObject for ViewModelBase to be " +
                                                    "use as its ViewModel.");

            Caliburn.Micro.Action.Invoke(this, "OnViewReady", (DependencyObject) view);
        }

        public virtual IEnumerable<IResult> OnViewReady()
        {
            yield break;
        }
    }
}