using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Caliburn.Micro;
using trello.Extensions;
using trello.Services.Handlers;
using trello.ViewModels;
using trello.Views.Cards;

namespace trello.Interactions
{
    public class DragHorizontalInteraction : InteractionBase
    {
        private const double MinimumDragDistance = 5.0;
        private const double FlickVelocity = 2000.0;

        private readonly ItemsControl _itemsControl;
        private readonly IEventAggregator _eventAggregator;
        private readonly DragImage _dragImage;

        private readonly BindableCollection<CardViewModel> _cardsModel;

        public DragHorizontalInteraction(DragImage dragImage, ItemsControl itemsControl,
                                         IEventAggregator eventAggregator)
        {
            _dragImage = dragImage;
            _itemsControl = itemsControl;
            _eventAggregator = eventAggregator;

            _cardsModel = (BindableCollection<CardViewModel>) itemsControl.ItemsSource;
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

            try
            {
                var element = sender as FrameworkElement;
                if (element == null || !HasPassedThresholds(element, e))
                    return;

                var item = (CardViewModel) ((FrameworkElement) sender).DataContext;
                var evt = new CardMovedToList
                {
                    CardId = item.Id,
                    Direction =
                        e.TotalManipulation.Translation.X < 0.0
                            ? ListMovementDirection.Left
                            : ListMovementDirection.Right
                };

                _cardsModel.Remove(item);
                _cardsModel.Refresh();

                // Notify the command handlers
                _eventAggregator.Publish(evt);
            }
            finally
            {
                IsActive = false;
                Complete();
            }
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
            var midpoint = Math.Abs(e.TotalManipulation.Translation.X) > element.ActualWidth/2;
            var velocity = Math.Abs(e.FinalVelocities.LinearVelocity.X) > FlickVelocity;
            return midpoint || velocity;
        }
    }
}