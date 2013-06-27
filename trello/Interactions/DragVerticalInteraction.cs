using System;
using System.Windows;
using System.Windows.Input;
using trello.Extensions;
using trello.Views.Cards;

namespace trello.Interactions
{
    public class DragVerticalInteraction : InteractionBase
    {
        private const double MinimumDragDistance = 5.0;
        private readonly UIElement _parent;
        private readonly DragImage _dragImage;

        public DragVerticalInteraction(DragImage dragImage, UIElement parent)
        {
            _dragImage = dragImage;
            _parent = parent;

            IsEnabled = true;
        }

        public override void AddElement(FrameworkElement element)
        {
            element.ManipulationDelta += HoldDelta;
            element.ManipulationCompleted += HoldCompleted;
        }

        private void HoldDelta(object sender, ManipulationDeltaEventArgs e)
        {
            if (ShouldIgnoreManipulation(e))
                return;

            // Avoid bubbling to any scroll viewers
            e.Handled = true;

            var dragTop = _dragImage.GetVerticalOffset().Value;
            var dragPotential = dragTop + e.DeltaManipulation.Translation.Y;
            if (dragPotential <= 0)
                dragPotential = 0;

            // Move the drag image
            _dragImage.SetVerticalOffset(dragPotential);
        }

        private void HoldCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            if (!IsActive)
                return;

            IsActive = false;

            // fade out the dragged image
            _dragImage.Animate(null, 0.0, UIElement.OpacityProperty, 700, 0,
                               completed: () => { _dragImage.Visibility = Visibility.Collapsed; });
        }

        private bool ShouldIgnoreManipulation(ManipulationDeltaEventArgs e)
        {
            if (!IsEnabled) return true;
            if (!IsActive && ShouldActivate(e))
            {
                IsActive = true;
            }
            return !IsActive;
        }

        private static bool ShouldActivate(ManipulationDeltaEventArgs e)
        {
            return Math.Abs(e.CumulativeManipulation.Translation.Y) >= MinimumDragDistance;
        }
    }
}
