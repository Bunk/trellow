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

        private readonly IEventAggregator _eventAggregator;
        private readonly string _previousListId;
        private readonly string _nextListId;
        private readonly DragImage _dragImage;

        private readonly BindableCollection<CardViewModel> _cardsModel;

        public DragHorizontalInteraction(DragImage dragImage,
                                         ItemsControl itemsControl,
                                         IEventAggregator eventAggregator,
                                         string previousListId,
                                         string nextListId)
        {
            _dragImage = dragImage;
            _eventAggregator = eventAggregator;
            _previousListId = previousListId;
            _nextListId = nextListId;

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

            var dragLeft = _dragImage.GetHorizontalOffset().Value;
            var dragPotential = dragLeft + e.DeltaManipulation.Translation.X;

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
                var evt = new CardMovingFromList
                {
                    Card = item.OriginalCard,
                    SourceListId = item.ListId,
                    DestinationListId = e.TotalManipulation.Translation.X < 0.0
                                            ? _previousListId
                                            : _nextListId
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

        private bool HasPassedThresholds(FrameworkElement element, ManipulationCompletedEventArgs e)
        {
            var midpoint = Math.Abs(e.TotalManipulation.Translation.X) > element.ActualWidth/2;
            var velocity = Math.Abs(e.FinalVelocities.LinearVelocity.X) > FlickVelocity;
            return midpoint || velocity;
        }
    }
}