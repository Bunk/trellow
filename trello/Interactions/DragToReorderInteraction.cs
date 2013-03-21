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
    public class DragToReorderInteraction : InteractionBase
    {
        private int _initialDragIndex;
        private FrameworkElement _initialDraggedItem;
        private int _lastDragIndex;
        private double _lastDragLocation;

        private readonly ItemsControl _list;
        private readonly DragImage _dragImage;
        private readonly IObservableCollection<CardViewModel> _cards;
        private readonly ScrollViewer _scrollViewer;
        private Panel _itemsPanel;

        public DragToReorderInteraction(ItemsControl list, DragImage dragImage)
        {
            _list = list;
            _dragImage = dragImage;
            _cards = list.ItemsSource as IObservableCollection<CardViewModel>;

            _scrollViewer = FindScrollViewer(list);

            IsEnabled = true;
        }

        public override void AddElement(FrameworkElement element)
        {
            element.Hold += HoldGesture;
            element.ManipulationDelta += HoldDelta;
            element.ManipulationCompleted += HoldCompleted;

            var position = element.GetRelativePositionIn(Application.Current.RootVisual);
            var relative = element.GetRelativePositionIn(_list);
            var index = GetPotentialIndex(element);

            Debug.WriteLine("Index: [{0}], Pos: [{1}], Rel: [{2}]", index, position, relative);
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
            _lastDragLocation = draggedPosition.Y;
        }

        private void HoldDelta(object sender, ManipulationDeltaEventArgs e)
        {
            if (!IsActive) return;

            // Avoid bubbling to any scroll viewers
            e.Handled = true;

            var top = _dragImage.GetVerticalOffset().Value;
            var mid = _dragImage.ActualHeight/2;
            var translation = e.DeltaManipulation.Translation.Y;
            var downward = translation > 0;
            var offset = top + translation;
            var potential = offset + mid;// +_scrollViewer.VerticalOffset;

            if (offset <= 0)
            {
                offset = 0;
                potential = 0;
            }

            var newIndex = GetPotentialLocation(_lastDragIndex, potential, downward);
            if (newIndex != _lastDragIndex)
            {
                _lastDragLocation = ShiftItems(newIndex);
                _lastDragIndex = newIndex;
            }

            // Move the drag image
            _dragImage.SetVerticalOffset(offset);
        }

        private void HoldCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            if (!IsActive) return;

            IsActive = false;

            // fade in the list
            _list.Animate(null, 1.0, UIElement.OpacityProperty, 200, 0);

            var dragIndex = _lastDragIndex;
            var targetLocation = _lastDragLocation - _scrollViewer.VerticalOffset;
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
            return elements.FirstOrDefault();
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
            var targetIndex = _list.Items.IndexOf(targetItem.DataContext);
            var potentialIndex = currentIndex;
            
            if (downwardMotion && dragPoint >= targetMid)
                potentialIndex = targetIndex; // target should be moved above the drag item

            if (!downwardMotion && dragPoint <= targetMid)
                potentialIndex = targetIndex; // target should be moved below the drag item

            Debug.WriteLine("Drag: [{0},{1}], Target: [{2},{3}], Potential: [{4}]",
                            dragPoint, currentIndex, targetMid, targetIndex, potentialIndex);

            return potentialIndex;
        }

        private double GetPotentialTop(int index)
        {
            // Note: Items may have shifted during flight, so use the previous index
            if (index <= 0) return 0;

            var element = _list.ItemContainerGenerator.ContainerFromIndex(index - 1) as FrameworkElement;
            if (element == null) return 0;

            var position = element.GetVerticalOffset().Value + _scrollViewer.VerticalOffset;
            //var position = element.GetRelativePositionIn(_list).Y + _scrollViewer.VerticalOffset;
            var size = element.ActualHeight;
            return position + size;
        }

        private double ShiftItems(int newIndex)
        {
            var offsetFromTop = 0.0;
            var offset = _dragImage.ActualHeight;
            for (var i = 0; i < _cards.Count; i++)
            {
                var item = _list.ItemContainerGenerator.ContainerFromIndex(i) as FrameworkElement;
                if (item == null) continue;

                // Calculate the total height above the new position
                var itemHeight = item.ActualHeight;
                if (i <= newIndex && i > _initialDragIndex)
                    offsetFromTop += itemHeight;
                else if (i < newIndex && i < _initialDragIndex)
                    offsetFromTop += itemHeight;

                // Shift the item up or down
                if (i <= newIndex && i > _initialDragIndex)
                    OffsetItem(-offset, item);
                else if (i >= newIndex && i < _initialDragIndex)
                    OffsetItem(offset, item);
                else
                    OffsetItem(0, item);
            }

            return offsetFromTop;
        }

        private void OffsetItem(double offset, FrameworkElement element)
        {
            var targetLocation = element.Tag != null ? (double) element.Tag : 0;
            if (targetLocation == offset) return;

            var transform = element.GetVerticalOffset().Transform;
            transform.Animate(null, offset, CompositeTransform.TranslateYProperty, 500, 0);

            element.Tag = offset;
        }
    }
}
