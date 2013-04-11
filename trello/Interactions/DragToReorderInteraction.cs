using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
    public class DragToReorderInteraction : InteractionBase
    {
        private const int AutoScrollHitRegionHeight = 80;

        private FrameworkElement _dragView;
        private int _initialDragIndex;
        private int _currentDragIndex;

        private readonly ItemsControl _list;
        private readonly DragImage _dragImage;
        private readonly IEventAggregator _eventAggregator;
        private readonly ScrollViewer _scrollViewer;
        private readonly DispatcherTimer _dispatcherTimer;
        private readonly BindableCollection<CardViewModel> _cards;
        private readonly List<IndexPoint> _indexMap;

        public DragToReorderInteraction(ItemsControl list, DragImage dragImage, IEventAggregator eventAggregator)
        {
            _list = list;
            _dragImage = dragImage;
            _eventAggregator = eventAggregator;
            _cards = list.ItemsSource as BindableCollection<CardViewModel>;
            _indexMap = new List<IndexPoint>();
            _scrollViewer = list.Descendants<ScrollViewer>().Cast<ScrollViewer>().Single();

            // setup the timer that solves movement and card relations
            _dispatcherTimer = new DispatcherTimer {Interval = TimeSpan.FromMilliseconds(50)};
            _dispatcherTimer.Tick += (sender, args) =>
            {
                AutoScrollLayout();
                ReorganizeLayout();
            };

            IsEnabled = true;
        }

        public override void AddElement(FrameworkElement element)
        {
            element.Hold += HoldGesture;
            element.ManipulationDelta += HoldDelta;
            element.ManipulationCompleted += HoldCompleted;

            var item = IndexPoint.Create(element, _list);
            _indexMap.Add(item);
            _indexMap.Sort((lhs, rhs) => lhs.Position.Top.CompareTo(rhs.Position.Top));
        }

        private void HoldGesture(object sender, GestureEventArgs e)
        {
            if (!IsEnabled)
                return;

            IsActive = true;

            // copy the dragged element into an image to visually move it
            _dragView = sender as CardView;
            if (_dragView == null)
            {
                IsActive = false;
                return;
            }

            // copy the drag image
            var draggedPosition = GetRelativePosition(_dragView);
            var bitmap = new WriteableBitmap(_dragView, null);
            _dragImage.Image.Source = bitmap;
            _dragImage.Visibility = Visibility.Visible;
            _dragImage.Opacity = 1.0;
            _dragImage.SetRotation(3);

            // this needs to be relative to the scrolled position
            _dragImage.SetVerticalOffset(draggedPosition.Y - _scrollViewer.VerticalOffset);

            // hide the underlying item
            _dragView.Opacity = 0.0;

            // fade out the list
            _list.Animate(1.0, 0.7, UIElement.OpacityProperty, 300, 0);

            _initialDragIndex = _currentDragIndex = IndexOf(draggedPosition);

            _dispatcherTimer.Start();
        }

        private void HoldDelta(object sender, ManipulationDeltaEventArgs e)
        {
            if (!IsActive) return;

            // Avoid bubbling to any scroll viewers
            e.Handled = true;

            var dragTop = _dragImage.GetVerticalOffset().Value;
            var dragMidpoint = _dragImage.ActualHeight/2;
            var dragPotential = dragTop + e.DeltaManipulation.Translation.Y;
            var intersectPotential = dragPotential + dragMidpoint + _scrollViewer.VerticalOffset;

            if (dragPotential <= 0)
            {
                dragPotential = 0;
                intersectPotential = 0;
            }

            var downward = e.DeltaManipulation.Translation.Y > 0;
            var newIndex = GetPotentialLocation(_currentDragIndex, intersectPotential, downward);
            if (_currentDragIndex != newIndex)
            {
                ReindexMap(_currentDragIndex, newIndex);
                _currentDragIndex = newIndex;
            }

            // Move the drag image
            _dragImage.SetVerticalOffset(dragPotential);
        }

        private void HoldCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            if (!IsActive)
                return;

            IsActive = false;
            _dispatcherTimer.Stop();

            // fade in the list
            _list.Animate(null, 1.0, UIElement.OpacityProperty, 200, 0);

            var dragIndex = _currentDragIndex;
            var targetItem = _indexMap[_currentDragIndex];
            var targetLocation = targetItem.Position.Top - _scrollViewer.VerticalOffset;
            var transform = _dragImage.GetVerticalOffset().Transform;
            transform.Animate(null, targetLocation, CompositeTransform.TranslateYProperty, 200, 0, completed: () =>
            {
                if (dragIndex != _initialDragIndex)
                {
                    // move the dragged item
                    var item = (CardViewModel) _dragView.DataContext;
                    _cards.Remove(item);
                    _cards.Insert(dragIndex, item);
                    _cards.Refresh();

                    // fire off the event for subscribers
                    _eventAggregator.Publish(CreateChangedEvent(item.Id, dragIndex, _cards.ToList()));
                }

                // reshow the hidden item
                _dragView.Opacity = 1.0;

                // fade out the dragged image
                _dragImage.Animate(null, 0.0, UIElement.OpacityProperty, 700, 0,
                                   completed: () => { _dragImage.Visibility = Visibility.Collapsed; });
            });
        }

        private static CardPriorityChanged CreateChangedEvent(string cardId, int index, IList<CardViewModel> cards)
        {
            var evt = new CardPriorityChanged
            {
                CardId = cardId
            };

            if (index == 0)
            {
                evt.Type = PositionType.Top;
            }
            else if (index == cards.Count - 1)
            {
                evt.Type = PositionType.Bottom;
            }
            else
            {
                var prev = cards[index - 1].Pos;
                var next = cards[index + 1].Pos;
                evt.Type = PositionType.Exact;
                evt.Pos = ((prev + next)/2);
            }
            return evt;
        }

        private Point GetRelativePosition(UIElement element)
        {
            var point = element.GetRelativePositionIn(_list);
            return new Point(point.X + _scrollViewer.HorizontalOffset, point.Y + _scrollViewer.VerticalOffset);
        }

        private int IndexOf(Point point)
        {
            var items = _indexMap.ToList();
            for (var i = 0; i < items.Count; i++)
            {
                if (items[i].Position.ContainsInclusive(point))
                    return i;
            }
            return -1;
        }

        private IndexPoint GetPotentialItem(double midPointRelativeToList)
        {
            var index = IndexOf(new Point(0, midPointRelativeToList));
            return index < 0 ? null : _indexMap[index];
        }

        private int GetPotentialLocation(int currentIndex, double dragPoint, bool downwardMotion)
        {
            var targetItem = GetPotentialItem(dragPoint);
            if (targetItem == null)
                return currentIndex;

            var targetPosition = targetItem.Position;
            var targetMid = targetPosition.Midpoint().Y;

            var targetIndex = _indexMap.IndexOf(targetItem);

            var potentialIndex = currentIndex;
            if (potentialIndex != targetIndex)
            {
                if (downwardMotion && dragPoint >= targetMid)
                    potentialIndex = targetIndex; // target should be moved above the drag item

                if (!downwardMotion && dragPoint <= targetMid)
                    potentialIndex = targetIndex; // target should be moved below the drag item
            }

            return potentialIndex;
        }

        private void ReindexMap(int oldIndex, int newIndex)
        {
            if (oldIndex == newIndex)
                throw new InvalidOperationException("You should only shift when indexes change.");

            if (oldIndex > newIndex)
                IntExtensions.Swap(ref oldIndex, ref newIndex);

            for (var i = oldIndex; i < newIndex; i++)
                SwapIndex(i, i + 1);

            _indexMap.Sort((lhs, rhs) => lhs.Position.Top.CompareTo(rhs.Position.Top));
        }

        private void SwapIndex(int indexFrom, int indexTo)
        {
            if (indexFrom == indexTo) return;

            if (indexFrom < 0)
                indexFrom = 0;

            if (indexTo < 0)
                indexTo = 0;

            if (indexFrom > indexTo)
                IntExtensions.Swap(ref indexFrom, ref indexTo);

            var itemA = _indexMap[indexFrom];
            var itemB = _indexMap[indexTo];

            var aTop = itemA.Position.Top + itemB.Position.Height;
            var bTop = itemA.Position.Top;

            itemA.Reposition(new Point(0, aTop));
            itemB.Reposition(new Point(0, bTop));
        }

        private void ReorganizeLayout()
        {
            var dragIndex = _currentDragIndex;

            // iterate over the items in the list and offset as required
            var offset = _dragImage.ActualHeight;
            for (var i = 0; i < _list.Items.Count; i++)
            {
                var item = _list.ItemContainerGenerator.ContainerFromIndex(i) as FrameworkElement;

                // determine which direction to offset this item by
                if (i <= dragIndex && i > _initialDragIndex)
                {
                    OffsetItem(-offset, item);
                }
                else if (i >= dragIndex && i < _initialDragIndex)
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
            var targetLocation = element.Tag != null ? (double) element.Tag : 0;
            if (Math.Abs(targetLocation - offset) < 0.01) return;

            var vertical = element.GetVerticalOffset();
            var transform = vertical.Transform;
            transform.Animate(null, offset, CompositeTransform.TranslateYProperty, 500, 0);

            element.Tag = offset;
        }

        private void AutoScrollLayout()
        {
            var dragLocation = _dragImage.GetRelativePositionIn(_list);
            var dragMidpoint = dragLocation.GetMidpoint(_dragImage.RenderSize);
            if (dragMidpoint.Y < AutoScrollHitRegionHeight)
            {
                // scroll up
                var velocity = AutoScrollHitRegionHeight - dragMidpoint.Y;
                var offset = _scrollViewer.VerticalOffset - velocity;
                _scrollViewer.ScrollToVerticalOffset(offset);
            }
            else if (dragMidpoint.Y > _list.ActualHeight - AutoScrollHitRegionHeight)
            {
                // scroll down
                var velocity = AutoScrollHitRegionHeight - (_list.ActualHeight - dragMidpoint.Y);
                var offset = _scrollViewer.VerticalOffset + velocity;
                _scrollViewer.ScrollToVerticalOffset(offset);
            }
        }

        private class IndexPoint
        {
            public Rect Position { get; private set; }

            public void Reposition(Point offset)
            {
                Position = new Rect(offset.X,
                                    offset.Y,
                                    Position.Width,
                                    Position.Height);
            }

            public static IndexPoint Create(FrameworkElement element, FrameworkElement relativeTo)
            {
                var position = element.GetRelativePositionIn(relativeTo);
                var rect = new Rect(position, element.RenderSize);
                return new IndexPoint {Position = rect};
            }
        }
    }
}