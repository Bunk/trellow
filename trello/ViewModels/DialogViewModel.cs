using System.Windows;

namespace trello.ViewModels
{
    public abstract class DialogViewModel : Caliburn.Micro.Screen
    {
        public UIElement Root { get; set; }

        public DialogViewModel() { }

        public DialogViewModel(object root)
        {
            var r = root as UIElement;
            if (r != null)
                Root = r;
        }

        protected override void OnActivate()
        {
            // disable underlying event handling
            if (Root != null)
                Root.IsHitTestVisible = false;
        }

        protected override void OnDeactivate(bool close)
        {
            //re-enable underlying event handling
            if (Root != null)
                Root.IsHitTestVisible = true;
        }
    }
}