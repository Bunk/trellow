using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using LinqToVisualTree;
using trello.Extensions;
using trello.ViewModels;
using trello.Views;
using trello.Views.Cards;

namespace trello.Interactions
{
    public class ReorderItem
    {
        public Object ViewModel { get; set; }

        public Rect Position { get; set; }

        public void Reposition(FrameworkElement element, Point offset)
        {
            var rect = new Rect(offset.X,
                                offset.Y,
                                Position.Width,
                                Position.Height);

            OffsetItem(element, offset.Y - Position.Y);

            Position = rect;

            //OffsetItem(element, rect.Top);
        }

        public static ReorderItem Create(FrameworkElement element, FrameworkElement relativeTo)
        {
            var position = element.GetRelativePositionIn(relativeTo);
            var rect = new Rect(position, element.RenderSize);
            var item = new ReorderItem
            {
                Position = rect,
                ViewModel = element.DataContext
            };
            return item;
        }

//        private void OffsetItem(FrameworkElement element, double y)
//        {
//            element.SetVerticalOffset(y);
//        }

        private void OffsetItem(FrameworkElement element, double offset)
        {
            // cache the location so that subsequent requests to move to the same location
            // are idempotent
            var targetLocation = element.Tag != null ? (double)element.Tag : 0;
            if (targetLocation == offset) return;

            var vertical = element.GetVerticalOffset();
            var transform = vertical.Transform;
            transform.Animate(null, offset, CompositeTransform.TranslateYProperty, 500, 0);

            element.Tag = offset;
        }
    }

    public class DragToReorderInteraction : InteractionBase
    {
        private FrameworkElement _draggedContainer;
        private CardViewModel _draggedModel;
        private int _initialDragIndex;
        private int _lastDragIndex;
        private double _lastDragLocation;
        private double _potentialLocationTop;

        private readonly ItemsControl _list;
        private readonly DragImage _dragImage;
        private readonly IObservableCollection<CardViewModel> _cards;
        private readonly ScrollViewer _scrollViewer;
        private Panel _itemsPanel;

        private readonly List<ReorderItem> _reorderItems;

        public DragToReorderInteraction(ItemsControl list, DragImage dragImage)
        {
            _list = list;
            _dragImage = dragImage;
            _cards = list.ItemsSource as IObservableCollection<CardViewModel>;
            _reorderItems = new List<ReorderItem>();
            _scrollViewer = FindScrollViewer(list);

            IsEnabled = true;
        }

        public override void AddElement(FrameworkElement element)
        {
            element.Hold += HoldGesture;
            element.ManipulationDelta += HoldDelta;
            element.ManipulationCompleted += HoldCompleted;

            var viewModel = element.DataContext;
            var container = (FrameworkElement) _list.ItemContainerGenerator.ContainerFromItem(viewModel);
            var item = ReorderItem.Create(container, _list);
            _reorderItems.Add(item);
            _reorderItems.Sort((lhs, rhs) => lhs.Position.Top.CompareTo(rhs.Position.Top));
        }

        private void PrintReorder()
        {
            for (var i = 0; i < _reorderItems.Count; i++)
                Debug.WriteLine("Index: [{0}], Pos: [{1}]", i, _reorderItems[i].Position);
        }

        private static ScrollViewer FindScrollViewer(ItemsControl list)
        {
            return list.Descendants<ScrollViewer>().Cast<ScrollViewer>().Single();
        }

        private void HoldGesture(object sender, GestureEventArgs e)
        {
            if (!IsEnabled) return;

            IsActive = true;

            if (_itemsPanel == null)
                _itemsPanel = FindPanel(_scrollViewer);

            // copy the dragged element into an image to visually move it
            var dragged = sender as FrameworkElement;
            if (dragged == null)
            {
                IsActive = false;
                return;
            }

            _draggedModel = (CardViewModel) dragged.DataContext;
            _draggedContainer = (FrameworkElement)_list.ItemContainerGenerator.ContainerFromItem(_draggedModel);
            //_draggedContainer.Opacity = 0.0;

            // copy the drag image
            var draggedPosition = dragged.GetRelativePositionIn(_list);
            var bitmap = new WriteableBitmap(dragged, null);
            _dragImage.Image.Source = bitmap;
            _dragImage.Visibility = Visibility.Visible;
            _dragImage.Opacity = 1.0;
            _dragImage.SetVerticalOffset(draggedPosition.Y);
            _dragImage.SetRotation(3);

            // fade out the list
            _list.Animate(1.0, 0.7, UIElement.OpacityProperty, 300, 0);

            _initialDragIndex = _cards.IndexOf(_draggedModel);
            _lastDragIndex = _initialDragIndex;
            _lastDragLocation = draggedPosition.GetMidpoint(dragged.RenderSize).Y;
            _potentialLocationTop = draggedPosition.Y;

            PrintReorder();
        }

        private void HoldDelta(object sender, ManipulationDeltaEventArgs e)
        {
            if (!IsActive) return;

            // Avoid bubbling to any scroll viewers
            e.Handled = true;

            var top = _dragImage.GetVerticalOffset().Value;
            var mid = _dragImage.ActualHeight/2;
            var translation = e.DeltaManipulation.Translation.Y;
            var offset = top + translation;
            var potential = offset + mid;// +_scrollViewer.VerticalOffset;

            if (offset <= 0)
            {
                offset = 0;
                potential = 0;
            }

            var downward = potential > _lastDragLocation;
            var newIndex = GetPotentialLocation(_lastDragIndex, potential, downward);
            if (_lastDragIndex != newIndex)
            {
                _potentialLocationTop = Shift(_lastDragIndex, newIndex);
                _lastDragIndex = newIndex;
            }

            // Move the drag image
            _dragImage.SetVerticalOffset(offset);
            _lastDragLocation = potential;
        }

        private void HoldCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            if (!IsActive) return;

            IsActive = false;

            PrintReorder();

            // fade in the list
            _list.Animate(null, 1.0, UIElement.OpacityProperty, 200, 0);

            var dragIndex = _lastDragIndex;
            var targetItem = _reorderItems[_lastDragIndex];
            var targetLocation = targetItem.Position.Top - _scrollViewer.VerticalOffset;
            var transform = _dragImage.GetVerticalOffset().Transform;
            transform.Animate(null, targetLocation, CompositeTransform.TranslateYProperty, 200, 0, completed: () =>
            {
                // move the dragged item
                _cards.Remove(_draggedModel);
                _cards.Insert(dragIndex, _draggedModel);
                _cards.Refresh();

                // reshow the hidden item
                _draggedContainer.Opacity = 1.0;

                // fade out the dragged image
                _dragImage.Animate(null, 0.0, UIElement.OpacityProperty, 700, 0, completed: () =>
                {
                    _dragImage.Visibility = Visibility.Collapsed;
                });
            });
        }

        private static Panel FindPanel(ScrollViewer scrollViewer)
        {
            var presenter = (ItemsPresenter) scrollViewer.Content;
            return (Panel) VisualTreeHelper.GetChild(presenter, 0);
        }

        private ReorderItem GetPotentialItem(double midPointRelativeToList)
        {
            var index = IndexOf(new Point(0, midPointRelativeToList));
            return index < 0 ? null : _reorderItems[index];
        }

        private int GetPotentialLocation(int currentIndex, double dragPoint, bool downwardMotion)
        {
            var targetItem = GetPotentialItem(dragPoint);
            if (targetItem == null)
            {
                return currentIndex;
            }

            var targetPosition = targetItem.Position;
            var targetVertical = targetPosition.Top + _scrollViewer.VerticalOffset;
            var targetMid = targetVertical + (targetPosition.Height/2);

            var targetIndex = _reorderItems.IndexOf(targetItem);
            
            var potentialIndex = currentIndex;

            if (potentialIndex != targetIndex)
            {
                if (downwardMotion && dragPoint >= targetMid)
                    potentialIndex = targetIndex; // target should be moved above the drag item

                if (!downwardMotion && dragPoint <= targetMid)
                    potentialIndex = targetIndex; // target should be moved below the drag item

                var targetName = ((CardViewModel)targetItem.ViewModel).Name;
                Debug.WriteLine("Drag [{0}], Target: [{1} '{2}'], Potential: [{3}], Motion: [{4}]",
                                dragPoint, targetMid, targetName, potentialIndex,
                                downwardMotion ? "Down" : "Up");
            }

            return potentialIndex;
        }

        private void ShiftItems(int indexFrom, int indexTo)
        {
            if (indexFrom == indexTo) return;

            if (indexFrom > indexTo)
                IntExtensions.Swap(ref indexFrom, ref indexTo);

            var itemA = _reorderItems[indexFrom];
            var itemB = _reorderItems[indexTo];

            var aTop = itemA.Position.Top + itemB.Position.Height;
            var bTop = itemA.Position.Top;

            var containerA = (FrameworkElement)_list.ItemContainerGenerator.ContainerFromItem(itemA.ViewModel);
            var containerB = (FrameworkElement)_list.ItemContainerGenerator.ContainerFromItem(itemB.ViewModel);

            var test = containerA.GetRelativePositionIn(_list).Y;
            var test2 = containerB.GetRelativePositionIn(_list).Y;

            Debug.WriteLine("[{0}] -> [{1}]", test, test2);

            itemA.Reposition(containerA, new Point(0, aTop));
            itemB.Reposition(containerB, new Point(0, bTop));
        }

        private double Shift(int oldIndex, int newIndex)
        {
            if (oldIndex == newIndex)
                throw new InvalidOperationException("You should only shift when indexes change.");

            if (oldIndex > newIndex)
                IntExtensions.Swap(ref oldIndex, ref newIndex);

            for (var i = oldIndex; i < newIndex; i++)
            {
                ShiftItems(i, i + 1);
            }

            _reorderItems.Sort((lhs, rhs) => lhs.Position.Top.CompareTo(rhs.Position.Top));

#if DEBUG
            PrintReorder();
#endif

            return _reorderItems.Take(newIndex).Sum(x => x.Position.Top);

//                for (var i = 0; i < _list.Items.Count; i++)
//                {
//                    var item = _list.ItemContainerGenerator.ContainerFromIndex(i) as FrameworkElement;
//                    if (item == null) continue;
//
//                    var position = item.GetRelativePositionIn(_list);
//
//                    // This doesn't work because we're actively modifying the collection in this loop
//                    // Need to cache the array
//                    var testpoint = item.GetRelativePositionIn(_list);
//                    testpoint.Y++;
//                    var itemIndex = IndexOf(cache, testpoint);
//
//                    // Calculate the total height above the new position
//                    var itemHeight = item.ActualHeight;
//                    if (itemIndex <= newIndex && itemIndex > oldIndex)
//                        offsetFromTop += itemHeight;
//                    else if (itemIndex < newIndex && itemIndex < oldIndex)
//                        offsetFromTop += itemHeight;
//
//                    // Shift the item up or down
//                    if (itemIndex <= newIndex && itemIndex > oldIndex)
//                    {
//                        OffsetItem(-offset, item);
//                        OffsetIndex(-offset, item);
//                    }
//                    else if (itemIndex >= newIndex && itemIndex < oldIndex)
//                    {
//                        OffsetItem(offset, item);
//                        OffsetIndex(offset, item);
//                    }
//                }
//
//            // Deal with the single moving card
//            _locationIndex[oldIndex] = placeholder;
//
//            _locationIndex = _locationIndex.OrderBy(x => x.Top).ToList();
//
//            return offsetFromTop;
        }

//        private void OffsetIndex(double offset, FrameworkElement element)
//        {
//            var vertical = element.GetVerticalOffset().Value;
//            var index = GetIndexOf(new Point(0, vertical));
//            if (index == -1) return;
//
//            _reorderItems[index].Reposition(new Point(0, offset));
//
//            _locationIndex[index] = new Rect(new Point(0, vertical + offset), element.RenderSize);
//        }

        private void OffsetItem(double offset, FrameworkElement element)
        {
            var targetLocation = element.Tag != null ? (double) element.Tag : 0;
            if (targetLocation == offset) return;

            var vertical = element.GetVerticalOffset();
            var transform = vertical.Transform;
            transform.Animate(null, offset, CompositeTransform.TranslateYProperty, 500, 0);

            element.Tag = offset;
        }

        private int IndexOf(Point point)
        {
            var items = _reorderItems.ToList();
            // basic hit test within a rectangle
            for (var i = 0; i < items.Count; i++)
            {
                if (items[i].Position.ContainsInclusive(point))
                    return i;
            }
            return -1;
        }
    }

    public static class RectExtensions
    {
        public static bool ContainsInclusive(this Rect rect, Point point)
        {
            if (point.X >= rect.Left && (rect.Left + rect.Width) > point.X &&
                point.Y >= rect.Top && (rect.Top + rect.Height) > point.Y)
                return true;
            return false;
        }
    }
}
