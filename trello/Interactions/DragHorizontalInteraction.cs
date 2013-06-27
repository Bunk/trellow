using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Caliburn.Micro;
using trello.Extensions;
using trello.ViewModels;
using trello.Views.Cards;

namespace trello.Interactions
{
    public class DragHorizontalInteraction : InteractionBase
    {
        private const double MinimumDragDistance = 5.0;
        private const double FlickVelocity = 2000.0;

        private readonly ItemsControl _itemsControl;
        private readonly DragImage _dragImage;

        private readonly BindableCollection<CardViewModel> _cardsModel;

        public DragHorizontalInteraction(DragImage dragImage, ItemsControl itemsControl)
        {
            _dragImage = dragImage;
            _itemsControl = itemsControl;

            _cardsModel = (BindableCollection<CardViewModel>)itemsControl.ItemsSource;
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
            if (!IsActive || !IsEnabled)
                return;

            var element = sender as FrameworkElement;
            if (element != null && HasPassedThresholds(element, e))
            {
                if (e.TotalManipulation.Translation.X < 0.0)
                {
                    // we went to the previous board, so raise that event
                }
                else
                {
                    // we went to the next board, so raise that event
                }

                // remove the item
                var item = (CardViewModel)((FrameworkElement)sender).DataContext;

                _cardsModel.Remove(item);
                _cardsModel.Refresh();
            }

            Complete();
            IsActive = false;
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
            else if (dragPotential >= _itemsControl.RenderSize.Width + 25)
            {
                dragPotential = _itemsControl.RenderSize.Width + 25;
            }
            return dragPotential;
        }

        private bool HasPassedThresholds(FrameworkElement element, ManipulationCompletedEventArgs e)
        {
            var midpoint = Math.Abs(e.TotalManipulation.Translation.X) > element.ActualWidth / 2;
            var velocity = Math.Abs(e.FinalVelocities.LinearVelocity.X) > FlickVelocity;
            return midpoint || velocity;
        }
    }
}
