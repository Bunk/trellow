using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Caliburn.Micro;
using LinqToVisualTree;
using trello.Extensions;
using trello.Services.Handlers;
using trello.ViewModels;
using trello.Views;
using trello.Views.Cards;

namespace trello.Interactions
{
    public class DragVerticalInteraction : InteractionBase
    {
        private const double MinimumDragDistance = 5.0;
        private const double AutoScrollHitRegionHeight = 80.0;
        
        private int _initialIndex;
        private int _currentIndex;
        private FrameworkElement _cardView;

        private readonly BindableCollection<CardViewModel> _cardsModel;

        private readonly ItemsControl _itemsControl;
        private readonly IEventAggregator _eventAggregator;
        private readonly DragImage _dragImage;
        private readonly ScrollViewer _scrollViewer;

        private readonly PointIndex _pointIndex;

        private readonly DispatcherTimer _dispatcherTimer;

        public DragVerticalInteraction(DragImage dragImage, ItemsControl itemsControl, IEventAggregator eventAggregator)
        {
            _dragImage = dragImage;
            _itemsControl = itemsControl;
            _eventAggregator = eventAggregator;
            _cardsModel = (BindableCollection<CardViewModel>) itemsControl.ItemsSource;
            _scrollViewer = itemsControl.Descendants<ScrollViewer>().Cast<ScrollViewer>().SingleOrDefault();
            Contract.Assume(_scrollViewer != null);

            // This indexes the cards and their positions in the list
            // so that we can more readily determine how to visually reorganize
            _pointIndex = new PointIndex();

            // setup the timer that solves movement and card relations
            _dispatcherTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
            _dispatcherTimer.Tick += (sender, args) =>
            {
                AutoScrollLayout();
                ReorganizeLayout();
            };
        }

        public override void AddElement(FrameworkElement element)
        {
            element.Hold += HoldStarted;
            element.ManipulationDelta += HoldDelta;
            element.ManipulationCompleted += HoldCompleted;

            var item = PointIndex.Value.Create(element, _itemsControl);
            _pointIndex.Add(item);
            _pointIndex.Sort((lhs, rhs) => lhs.Position.Top.CompareTo(rhs.Position.Top));
        }

        private void HoldStarted(object sender, GestureEventArgs e)
        {
            if (ShouldIgnoreManipulation())
                return;

            _cardView = sender as CardView;
            if (_cardView == null)
            {
                IsActive = false;
            }
            else
            {
                var scrollOffset = new Point(_scrollViewer.HorizontalOffset, _scrollViewer.VerticalOffset);
                var originalPosition = _cardView.GetRelativePositionIn(_itemsControl, scrollOffset);
                _initialIndex = _currentIndex = _pointIndex.IndexOf(originalPosition);
            }
        }

        private void HoldDelta(object sender, ManipulationDeltaEventArgs e)
        {
            if (ShouldIgnoreManipulation(e))
                return;

            // Avoid bubbling to any scroll viewers
            e.Handled = true;

            var dragTop = _dragImage.GetVerticalOffset().Value;
            var dragMid = _dragImage.ActualHeight/2;
            var potentialY = dragTop + e.DeltaManipulation.Translation.Y;
            var potentialIntersect = potentialY + dragMid + _scrollViewer.VerticalOffset;
            if (potentialY <= 0)
            {
                potentialY = 0;
                potentialIntersect = 0;
            }

            var isDownward = e.DeltaManipulation.Translation.Y > 0;
            var potentialIndex = GetPotentialIndex(_currentIndex, potentialIntersect, isDownward);
            if (_currentIndex != potentialIndex)
            {
                // We only want to reindex if the index we've moved the card to 
                // has changed, based on hardcore maths.
                _pointIndex.Reindex(_currentIndex, potentialIndex);
                _currentIndex = potentialIndex;
            }

            // Move the drag image
            _dragImage.SetVerticalOffset(potentialY);
        }

        private void HoldCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            if (!IsActive)
                return;

            // stop the timer so that we don't try to re-fix this thing after moving to our
            // final destination.
            _dispatcherTimer.Stop();

            var dragIndex = _currentIndex;
            var targetItem = _pointIndex.Get(_currentIndex);
            var targetLocation = targetItem.Position.Top - _scrollViewer.VerticalOffset;
            var transform = _dragImage.GetVerticalOffset().Transform;
            transform.Animate(null, targetLocation, CompositeTransform.TranslateYProperty, 200, 0, completed: () =>
            {
                Complete();
                if (dragIndex == _initialIndex) 
                    return;

                // move the dragged item
                var item = (CardViewModel)_cardView.DataContext;

                _cardsModel.Remove(item);
                _cardsModel.Insert(dragIndex, item);
                _cardsModel.Refresh();

                // fire off the event for subscribers
                _eventAggregator.Publish(CardPriorityChanged.Create(item.Id, dragIndex, _cardsModel.ToList()));
            });

            IsActive = false;
        }

        private bool ShouldIgnoreManipulation()
        {
            return !IsEnabled && !IsActive;
        }

        private bool ShouldIgnoreManipulation(ManipulationDeltaEventArgs e)
        {
            if (!IsEnabled) return true;
            if (!IsActive && ShouldActivate(e))
            {
                IsActive = true;
                _dispatcherTimer.Start();
            }
            return !IsActive;
        }

        private static bool ShouldActivate(ManipulationDeltaEventArgs e)
        {
            return Math.Abs(e.CumulativeManipulation.Translation.Y) >= MinimumDragDistance;
        }

        private void AutoScrollLayout()
        {
            var dragLocation = _dragImage.GetRelativePositionIn(_itemsControl);
            var dragMidpoint = dragLocation.GetMidpoint(_dragImage.RenderSize);
            if (dragMidpoint.Y < AutoScrollHitRegionHeight)
            {
                // scroll up
                var velocity = AutoScrollHitRegionHeight - dragMidpoint.Y;
                var offset = _scrollViewer.VerticalOffset - velocity;
                _scrollViewer.ScrollToVerticalOffset(offset);
            }
            else if (dragMidpoint.Y > _itemsControl.ActualHeight - AutoScrollHitRegionHeight)
            {
                // scroll down
                var velocity = AutoScrollHitRegionHeight - (_itemsControl.ActualHeight - dragMidpoint.Y);
                var offset = _scrollViewer.VerticalOffset + velocity;
                _scrollViewer.ScrollToVerticalOffset(offset);
            }
        }

        private void ReorganizeLayout()
        {
            var dragIndex = _currentIndex;

            // iterate over the items in the list and offset as required
            var offset = _dragImage.ActualHeight;
            for (var i = 0; i < _itemsControl.Items.Count; i++)
            {
                var item = _itemsControl.ItemContainerGenerator.ContainerFromIndex(i) as FrameworkElement;
                if (item == null)
                    break;

                // determine which direction to offset this item by
                if (i <= dragIndex && i > _initialIndex)
                {
                    OffsetItem(-offset, item);
                }
                else if (i >= dragIndex && i < _initialIndex)
                {
                    OffsetItem(offset, item);
                }
                else
                {
                    OffsetItem(0, item);
                }
            }
        }

        private static void OffsetItem(double offset, FrameworkElement element)
        {
            var targetLocation = element.Tag != null ? (double)element.Tag : 0;
            if (Math.Abs(targetLocation - offset) < 0.01) return;

            var vertical = element.GetVerticalOffset();
            var transform = vertical.Transform;
            transform.Animate(null, offset, CompositeTransform.TranslateYProperty, 500, 0);

            element.Tag = offset;
        }

        private int GetPotentialIndex(int currentIndex, double dragPoint, bool downwardMotion)
        {
            var targetItem = _pointIndex.GetPotentialItem(dragPoint);
            if (targetItem == null)
                return currentIndex;

            var targetPosition = targetItem.Position;
            var targetMid = targetPosition.Midpoint().Y;
            var targetIndex = _pointIndex.IndexOf(targetItem);

            var potentialIndex = currentIndex;
            if (potentialIndex != targetIndex)
            {
                if (downwardMotion && dragPoint >= targetMid)
                    potentialIndex = targetIndex; // move target above the dragged item
                if (!downwardMotion && dragPoint <= targetMid)
                    potentialIndex = targetIndex; // move target below the drag item
            }

            return potentialIndex;
        }
    }
}
