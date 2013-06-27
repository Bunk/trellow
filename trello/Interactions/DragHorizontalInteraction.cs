using System;
using System.Windows;
using System.Windows.Input;
using trello.Extensions;
using trello.Views.Cards;

namespace trello.Interactions
{
    public class DragHorizontalInteraction : InteractionBase
    {
        private const double MinimumDragDistance = 5.0;
        private readonly UIElement _context;
        private readonly DragImage _dragImage;

        public DragHorizontalInteraction(DragImage dragImage, UIElement context)
        {
            _dragImage = dragImage;
            _context = context;

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

            var dragTop = _dragImage.GetHorizontalOffset().Value;
            var dragPotential = dragTop + e.DeltaManipulation.Translation.X;
            dragPotential = ConstrainDrag(dragPotential);

            // Move the drag image
            _dragImage.SetHorizontalOffset(dragPotential);
        }

        private void HoldCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            if (!IsActive)
                return;

            IsActive = false;

            Complete();
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
            return Math.Abs(e.CumulativeManipulation.Translation.X) >= MinimumDragDistance;
        }

        private double ConstrainDrag(double dragPotential)
        {
            if (dragPotential <= 0)
            {
                dragPotential = 0;
            }
            else if (dragPotential >= _context.RenderSize.Width + 25)
            {
                dragPotential = _context.RenderSize.Width + 25;
            }
            return dragPotential;
        }
    }
}
