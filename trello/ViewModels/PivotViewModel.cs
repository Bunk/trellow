using Caliburn.Micro;
using Microsoft.Phone.Shell;

namespace trello.ViewModels
{
    public interface ISetupTheAppBar
    {
        ApplicationBar SetupTheAppBar(ApplicationBar existing);
    }

    public abstract class PivotViewModel : Conductor<IScreen>.Collection.OneActive
    {
        private readonly PivotFix<IScreen> _pivotFix;
        private ApplicationBar _appBar;

        public ApplicationBar AppBar
        {
            get { return _appBar; }
            set
            {
                _appBar = value;
                NotifyOfPropertyChange(() => AppBar);
            }
        }

        public PivotViewModel()
        {
            _pivotFix = new PivotFix<IScreen>(this);

            AppBar = new ApplicationBar {IsVisible = true, IsMenuEnabled = true, Opacity = 1};
        }

        protected override void OnViewLoaded(object view)
        {
            _pivotFix.OnViewLoaded(view, base.OnViewLoaded);
        }

        protected override void ChangeActiveItem(IScreen newItem, bool closePrevious)
        {
            AppBar.IsVisible = false;
            NotifyOfPropertyChange(() => AppBar);

            _pivotFix.ChangeActiveItem(newItem, closePrevious, base.ChangeActiveItem);
        }

        protected override void OnActivationProcessed(IScreen item, bool success)
        {
            base.OnActivationProcessed(item, success);

            var appbar = BuildDefaultAppBar();

            var screen = item as ISetupTheAppBar;
            if (screen != null)
            {
                appbar = screen.SetupTheAppBar(appbar);
            }

            AppBar = appbar;
        }

        protected virtual ApplicationBar BuildDefaultAppBar()
        {
            var bar = new ApplicationBar { IsVisible = true, IsMenuEnabled = true, Opacity = 1 };

            return bar;
        }
    }
}