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
        public FrameworkElement Element { get; set; }

        public Rect Position { get; set; }

        public void Reposition(Point offset)
        {
            var rect = new Rect(Position.X + offset.X,
                                Position.Y + offset.Y,
                                Position.Width,
                                Position.Height);
            Position = rect;
        }

        public static ReorderItem Create(FrameworkElement element, FrameworkElement relativeTo)
        {
            var position = element.GetRelativePositionIn(relativeTo);
            var rect = new Rect(position, element.RenderSize);
            var item = new ReorderItem
            {
                Position = rect,
                Element = element
            };
            return item;
        }
    }

    public class DragToReorderInteraction : InteractionBase
    {
        private FrameworkElement _initialDraggedItem;
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

        private static void PrintReorderList(IDictionary<FrameworkElement, int> items)
        {
            Debug.WriteLine("----------------------");
            foreach (var kvp in items)
            {
                var val = ((CardViewModel) kvp.Key.DataContext).Name;
                Debug.WriteLine("[{0}] - {1}", kvp.Value, val);
            }
        }

        public override void AddElement(FrameworkElement element)
        {
            element.Hold += HoldGesture;
            element.ManipulationDelta += HoldDelta;
            element.ManipulationCompleted += HoldCompleted;

            var item = ReorderItem.Create(element, _list);
            _reorderItems.Add(item);
            _reorderItems.Sort((lhs, rhs) => lhs.Position.Top.CompareTo(rhs.Position.Top));

            var index = GetIndexOf(new Point(item.Position.Left, item.Position.Top));

            Debug.WriteLine("Index: [{0}], Pos: [{1}]", index, item.Position);
        }

        private int GetIndexOf(Point point)
        {
            for (var i = 0; i < _reorderItems.Count; i++)
            {
                if (_reorderItems[i].Position.ContainsInclusive(point))
                    return i;
            }
            return -1;
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
            _initialDraggedItem = sender as FrameworkElement;
            if (_initialDraggedItem == null)
            {
                IsActive = false;
                return;
            }

            // copy the drag image
            var draggedPosition = _initialDraggedItem.GetRelativePositionIn(_list);
            var bitmap = new WriteableBitmap(_initialDraggedItem, null);
            _dragImage.Image.Source = bitmap;
            _dragImage.Visibility = Visibility.Visible;
            _dragImage.Opacity = 1.0;
            _dragImage.SetVerticalOffset(draggedPosition.Y);
            //_dragImage.SetRotation(5);

            // hide it
            _initialDraggedItem.Opacity = 0.0;

            // fade out the list
            _list.Animate(1.0, 0.7, UIElement.OpacityProperty, 300, 0);

            _initialDragIndex = _cards.IndexOf((CardViewModel) _initialDraggedItem.DataContext);
            _lastDragIndex = _initialDragIndex;
            _lastDragLocation = draggedPosition.GetMidpoint(_initialDraggedItem.RenderSize).Y;
            _potentialLocationTop = draggedPosition.Y;
            _locationIndex = Reindex(_list);
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
            if (newIndex != _lastDragIndex)
            {
                _potentialLocationTop = ShiftItems(newIndex);
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

            // fade in the list
            _list.Animate(null, 1.0, UIElement.OpacityProperty, 200, 0);

            var dragIndex = _lastDragIndex;
            var targetLocation = _potentialLocationTop - _scrollViewer.VerticalOffset;
            var transform = _dragImage.GetVerticalOffset().Transform;
            transform.Animate(null, targetLocation, CompositeTransform.TranslateYProperty, 200, 0, completed: () =>
            {
                // move the dragged item
                var draggedItem = _cards[_initialDragIndex];
                _cards.Remove(draggedItem);
                _cards.Insert(dragIndex, draggedItem);

                _cards.Refresh();

                if (_initialDraggedItem != null)
                {
                    _initialDraggedItem.Opacity = 1.0;
                    _initialDraggedItem = null;
                }

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

        private int GetPotentialIndex(FrameworkElement item)
        {
            return _list.Items.IndexOf(item.DataContext);
        }

        private CardView GetPotentialItem(double midPointRelativeToList)
        {
            var listPosition = _list.GetRelativePositionIn(Application.Current.RootVisual);
            var targetPoint = new Point(listPosition.X, listPosition.Y + midPointRelativeToList);
            var elements = VisualTreeHelper
                .FindElementsInHostCoordinates(targetPoint, Application.Current.RootVisual)
                .OfType<CardView>();

            // in the case of overlapping cards, we want to return the furthest below card
            return elements.FirstOrDefault(e => e != _initialDraggedItem);
        }

        private int GetPotentialLocation(int currentIndex, double dragPoint, bool downwardMotion)
        {
            var targetItem = GetPotentialItem(dragPoint);
            if (targetItem == null)
            {
                return currentIndex;
            }

            var targetPosition = targetItem.GetRelativePositionIn(_list);
            var targetVertical = targetPosition.Y + _scrollViewer.VerticalOffset;
            var targetMid = targetVertical + (targetItem.ActualHeight/2);
            var targetBot = targetVertical + targetItem.ActualHeight + 1;
            
            // this isn't returning the correct index if you go up and come back down
            //var targetIndex = _list.Items.IndexOf(targetItem.DataContext);
            //var targetIndex = FindVisualIndex(targetItem);
            var targetIndex = GetItemIndex(targetItem);
            
            var potentialIndex = currentIndex;

            if (downwardMotion && dragPoint >= targetMid)
                potentialIndex = targetIndex; // target should be moved above the drag item

            if (!downwardMotion && dragPoint <= targetMid)
                potentialIndex = targetIndex; // target should be moved below the drag item

            var targetName = ((CardViewModel) targetItem.DataContext).Name;
            Debug.WriteLine("Drag [{0}], Target: [{1} '{2}'], Potential: [{3}], Motion: [{4}]",
                            dragPoint, targetMid, targetName, potentialIndex,
                            downwardMotion ? "Down" : "Up");

            return potentialIndex;
        }

        private int GetItemIndex(CardView targetItem)
        {
            var targetMid = targetItem.GetRelativePositionIn(_list).Y +
                            _scrollViewer.VerticalOffset +
                            targetItem.ActualHeight/2;

            return IndexOf(_locationIndex, new Point(0, targetMid));
        }

        //private IDictionary<string, int> _locationIndex;

        private IList<Rect> _locationIndex;

//        private static IDictionary<string, int> Reindex(ItemsControl control)
//        {
//            var index = new Dictionary<string, int>();
//
//            var cards = control.Items.OfType<CardViewModel>()
//                .Select((card, i) =>
//                {
//                    var item = (FrameworkElement) control.ItemContainerGenerator.ContainerFromIndex(i);
//                    return new
//                    {
//                        Id = ((CardViewModel) item.DataContext).Id,
//                        Area = new Rect(item.GetRelativePositionIn(control), item.RenderSize)
//                    };
//                })
//                .OrderBy(x => x.Area.Top)
//                .ToList();
//
//            for (var i = 0; i < cards.Count; i++)
//            {
//                var card = cards[i];
//                index[card.Id] = i;
//            }
//
//            return index;
//        }

        private static IList<Rect> Reindex(ItemsControl control)
        {
            var list = new List<Rect>();
            for (var i = 0; i < control.Items.Count; i++)
            {
                var item = (FrameworkElement) control.ItemContainerGenerator.ContainerFromIndex(i);
                var position = item.GetRelativePositionIn(control);
                list.Add(new Rect(position, item.RenderSize));
            }
            return list.OrderBy(x => x.Top).ToList();
        }

        private double ShiftItems(int newIndex)
        {
            var offsetFromTop = 0.0;
            var offset = _dragImage.ActualHeight;
            var placeholder = _locationIndex[newIndex];

            var cache = new Rect[_locationIndex.Count];
            _locationIndex.CopyTo(cache, 0);

            for (var i = 0; i < _list.Items.Count; i++)
            {
                var item = _list.ItemContainerGenerator.ContainerFromIndex(i) as FrameworkElement;
                if (item == null) continue;

                // This doesn't work because we're actively modifying the collection in this loop
                // Need to cache the array
                var testpoint = item.GetRelativePositionIn(_list);
                testpoint.Y++;
                var itemIndex = IndexOf(cache, testpoint);

                // Calculate the total height above the new position
                var itemHeight = item.ActualHeight;
                if (itemIndex <= newIndex && itemIndex > _lastDragIndex)
                    offsetFromTop += itemHeight;
                else if (itemIndex < newIndex && itemIndex < _lastDragIndex)
                    offsetFromTop += itemHeight;

                // Shift the item up or down
                if (itemIndex <= newIndex && itemIndex > _lastDragIndex)
                {
                    OffsetItem(-offset, item);
                    OffsetIndex(-offset, item);
                }
                else if (itemIndex >= newIndex && itemIndex < _lastDragIndex)
                {
                    OffsetItem(offset, item);
                    OffsetIndex(offset, item);
                }
            }

            // Deal with the single moving card
            _locationIndex[_lastDragIndex] = placeholder;

            _locationIndex = _locationIndex.OrderBy(x => x.Top).ToList();

            return offsetFromTop;
        }

        private void OffsetIndex(double offset, FrameworkElement element)
        {
            var vertical = element.GetVerticalOffset().Value;
            var index = IndexOf(_locationIndex, new Point(0, vertical));
            if (index == -1) return;

            _locationIndex[index] = new Rect(new Point(0, vertical + offset), element.RenderSize);
        }

        private void OffsetItem(double offset, FrameworkElement element)
        {
            var targetLocation = element.Tag != null ? (double) element.Tag : 0;
            if (targetLocation == offset) return;

            var vertical = element.GetVerticalOffset();
            var transform = vertical.Transform;
            transform.Animate(null, offset, CompositeTransform.TranslateYProperty, 500, 0);

            element.Tag = offset;
        }

        private int IndexOf(IList<Rect> list, Point point)
        {
            // basic hit test within a rectangle
            for (var i = 0; i <= list.Count; i++)
            {
                if (list[i].ContainsInclusive(point))
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
