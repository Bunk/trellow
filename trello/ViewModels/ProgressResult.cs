using Caliburn.Micro;
using Microsoft.Phone.Shell;

namespace trello.ViewModels
{
    public class ProgressResult : ResultBase
    {
        private readonly bool _visible;
        private readonly string _text;

        public ProgressResult(bool visible = false, string text = "")
        {
            _visible = visible;
            _text = text;
        }

        public override void Execute(ActionExecutionContext context)
        {
            var progressIndicator = SystemTray.GetProgressIndicator(context.View);
            if (progressIndicator == null)
            {
                progressIndicator = new ProgressIndicator();
                SystemTray.SetProgressIndicator(context.View, progressIndicator);
            }

            progressIndicator.IsVisible = _visible;
            progressIndicator.Text = _text;
            progressIndicator.IsIndeterminate = true;

            OnCompleted();
        }
    }
}