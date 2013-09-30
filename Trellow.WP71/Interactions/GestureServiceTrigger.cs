using System.Windows;
using System.Windows.Interactivity;
#if WP7
using Microsoft.Phone.Controls;
#else
using GestureEventArgs = System.Windows.Input.GestureEventArgs;
#endif

namespace Trellow.Interactions
{
    public enum Gesture
    {
        Tap,
        Hold,
        DoubleTap
    }

    public class GestureServiceTrigger : TriggerBase<FrameworkElement>
    {
        public Gesture Gesture { get; set; }

        protected override void OnAttached()
        {
            // note: Deprecated for WP8+... necessary for WP7-
#if WP7
            var listener = GestureService.GetGestureListener(AssociatedObject);
#else
            var listener = AssociatedObject;
#endif
            switch (Gesture)
            {
                case Gesture.Tap:
                    listener.Tap += OnGesture;
                    break;
                case Gesture.Hold:
                    listener.Hold += OnGesture;
                    break;
                case Gesture.DoubleTap:
                    listener.DoubleTap += OnGesture;
                    break;
            }
        }

        protected override void OnDetaching()
        {
#if WP7
            var listener = GestureService.GetGestureListener(AssociatedObject);
#else
            var listener = AssociatedObject;
#endif
            switch (Gesture)
            {
                case Gesture.Tap:
                    listener.Tap -= OnGesture;
                    break;
                case Gesture.Hold:
                    listener.Hold -= OnGesture;
                    break;
                case Gesture.DoubleTap:
                    listener.DoubleTap -= OnGesture;
                    break;
            }
        }

        private void OnGesture(object sender, GestureEventArgs e)
        {
            InvokeActions(e);
            e.Handled = true;
        }
    }
}