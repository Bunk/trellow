using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace trello.ViewModels
{
    public class ProgressService : IProgressService
    {
        private const string DefaultIndicatorText = "Loading";
        private readonly ProgressIndicator _indicator;

        public ProgressService(PhoneApplicationFrame rootFrame)
        {
            _indicator = new ProgressIndicator {Text = DefaultIndicatorText};
            rootFrame.Navigated += RootFrameOnNavigated;
        }

        public void Show()
        {
            Show(DefaultIndicatorText);
        }

        public void Show(string text)
        {
            _indicator.Text = text;
            _indicator.IsVisible = true;
            _indicator.IsIndeterminate = true;
        }

        public void Hide()
        {
            _indicator.IsIndeterminate = false;
            _indicator.IsVisible = false;
        }

        private void RootFrameOnNavigated(object sender, NavigationEventArgs args)
        {
            var content = args.Content as PhoneApplicationPage;
            if (content == null)
                return;

            content.SetValue(SystemTray.ProgressIndicatorProperty, _indicator);
        }
    }
}