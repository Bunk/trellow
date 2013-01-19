﻿using System.Windows;
using System.Windows.Interactivity;
using Microsoft.Phone.Controls;

public enum Gesture
{
    Tap,
    Hold,
    DoubleTap
}

namespace trello.Views.Triggers
{
    public class GestureServiceTrigger : TriggerBase<FrameworkElement>
    {
        public Gesture Gesture { get; set; }

        protected override void OnAttached()
        {
            var listener = GestureService.GetGestureListener(AssociatedObject);

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
            var listener = GestureService.GetGestureListener(AssociatedObject);
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