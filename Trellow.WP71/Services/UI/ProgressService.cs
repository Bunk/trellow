using System.Threading;
using System.Windows.Navigation;
using JetBrains.Annotations;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Trellow.Services.UI
{
    [UsedImplicitly]
    public class ProgressService : IProgressService
    {
        private const string DefaultIndicatorText = "Loading";
        private readonly ProgressIndicator _indicator;
        private int _count;

        public ProgressService(PhoneApplicationFrame rootFrame)
        {
            _indicator = new ProgressIndicator {Text = DefaultIndicatorText};

            rootFrame.Navigated += RootFrameOnNavigated;

            UpdateProgressBarAttachment(rootFrame.Content);
        }

        public void Show()
        {
            Show(DefaultIndicatorText);
        }

        public void Show(string text)
        {
            Interlocked.Increment(ref _count);

            _indicator.Text = text;
            _indicator.IsVisible = true;
            _indicator.IsIndeterminate = true;
        }

        public void Hide()
        {
            Interlocked.Decrement(ref _count);

            if (_count != 0) return;

            _indicator.IsIndeterminate = false;
            _indicator.IsVisible = false;
        }

        private void UpdateProgressBarAttachment(object obj)
        {
            var content = obj as PhoneApplicationPage;
            if (content == null)
                return;

            content.SetValue(SystemTray.ProgressIndicatorProperty, _indicator);
        }

        private void RootFrameOnNavigated(object sender, NavigationEventArgs args)
        {
            UpdateProgressBarAttachment(args.Content);
        }
    }
}