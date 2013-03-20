using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Caliburn.Micro;
using LinqToVisualTree;
using trello.Extensions;
using trello.ViewModels;
using trello.Views.Cards;

namespace trello.Interactions
{
    public class DragToReorderInteraction : InteractionBase
    {
        private int _initialDragIndex;
        private int _lastDragIndex;
        private FrameworkElement _initialDraggedItem;

        private readonly ItemsControl _list;
        private readonly DragImage _dragImage;
        private readonly IObservableCollection<CardViewModel> _cards;
        private readonly DispatcherTimer _autoScrollTimer;
        private readonly ScrollViewer _scrollViewer;
        private Panel _itemsPanel;

        private Action<int> _indexChanged; 

        public DragToReorderInteraction(ItemsControl list, DragImage dragImage)
        {
            _list = list;
            _dragImage = dragImage;
            _cards = list.ItemsSource as IObservableCollection<CardViewModel>;

            _scrollViewer = FindScrollViewer(list);

            _autoScrollTimer = new DispatcherTimer {Interval = TimeSpan.FromMilliseconds(50)};
            _autoScrollTimer.Tick += (sender, args) =>
            {
                //ShuffleItems();
            };

            _indexChanged = i =>
            {
                Debug.WriteLine("Index changed to: {0}", i);
                _lastDragIndex = i;
                ShuffleItems();
            };

            IsEnabled = true;
        }

        public override void AddElement(FrameworkElement element)
        {
            element.Hold += HoldGesture;
            element.ManipulationDelta += HoldDelta;
            element.ManipulationCompleted += HoldCompleted;
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
            UpdateDragImage(_initialDraggedItem);

            // hide it
            _initialDraggedItem.Opacity = 0.0;

            // fade out the list
            _list.Animate(1.0, 0.7, UIElement.OpacityProperty, 300, 0);

            _initialDragIndex = _cards.IndexOf((CardViewModel) _initialDraggedItem.DataContext);
            _lastDragIndex = _initialDragIndex;

            _autoScrollTimer.Start();
        }

        private void HoldDelta(object sender, ManipulationDeltaEventArgs e)
        {
            if (!IsActive) return;

            // Avoid bubbling to any scroll viewers
            e.Handled = true;

            var top = _dragImage.GetVerticalOffset().Value;
            var offset = top + e.DeltaManipulation.Translation.Y;
            var heightDelta = _initialDraggedItem.ActualHeight - _dragImage.ActualHeight;

            if (offset < 0) // At the top of the list
            {
                offset = 0;
                UpdateDropTarget(offset);
            }
            else
            {
                // Check at the midpoint of the drag image
                UpdateDropTarget(offset + _dragImage.ActualHeight/2);
            }

            // Move the drag image
            _dragImage.SetVerticalOffset(offset);
        }

        private void HoldCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            if (!IsActive) return;

            IsActive = false;
            _autoScrollTimer.Stop();

            // fade in the list
            _list.Animate(null, 1.0, UIElement.OpacityProperty, 200, 0);

            // animate the dragged item into place
            var dragIndex = GetPotentialDragIndex();
            var targetLocation = GetTotalHeightAbove(dragIndex) - _scrollViewer.VerticalOffset;
            //var targetLocation = dragIndex*_initialDraggedItem.ActualHeight - _scrollViewer.VerticalOffset;
            var transform = _dragImage.GetVerticalOffset().Transform;
            transform.Animate(null, targetLocation, TranslateTransform.YProperty, 200, 0, completed: () =>
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
                _dragImage.Animate(null, 0.0, UIElement.OpacityProperty, 1000, 0, completed: () =>
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

        private void UpdateDropTarget(double targetOffset)
        {
            var original = _initialDraggedItem.GetRelativePositionIn(_list);
            var targetPosition = new Point(original.X, targetOffset);
            var elements = VisualTreeHelper.FindElementsInHostCoordinates(targetPosition, _itemsPanel);
            var targetItem = elements.FirstOrDefault();
            if (targetItem == null) return;


        }

        private void UpdateDragImage(UIElement draggedItem)
        {
            // todo: rotate the item like they do on trello.com
            var bitmap = new WriteableBitmap(draggedItem, null);
            _dragImage.Image.Source = bitmap;
            _dragImage.Visibility = Visibility.Visible;
            _dragImage.Opacity = 1.0;
            _dragImage.SetVerticalOffset(draggedItem.GetRelativePositionIn(_list).Y);
        }

        private int GetPotentialDragIndex()
        {
            var dragIndex = 0;
            var position = _dragImage.GetRelativePositionIn(_list).Y + _scrollViewer.VerticalOffset;
            var midpoint = position + (_dragImage.ActualHeight/2);

            for (var i = 0; i < _cards.Count; i++)
            {
                var currCard = (FrameworkElement) _list.ItemContainerGenerator.ContainerFromIndex(i);
                var currCardTop = currCard.GetRelativePositionIn(_list).Y + _scrollViewer.VerticalOffset;
                var currCardBot = currCardTop + currCard.ActualHeight;
                
                if (midpoint >= currCardTop && midpoint < currCardBot)
                {
                    Debug.WriteLine("Drag: [{0}], Top: [{1}], Bottom: [{2}], Index: [{3}]", midpoint,
                                    currCardTop, currCardBot, i);
                    dragIndex = i;
                    break;
                }
            }

            return dragIndex;
        }

        private double GetTotalHeightAbove(int index)
        {
            if (index <= 0) return 0;

            var element = _list.ItemContainerGenerator.ContainerFromIndex(index - 1) as FrameworkElement;
            if (element == null) return 0;

            var position = element.GetRelativePositionIn(_list).Y;
            var size = element.ActualHeight;
            return position + size;
        }

        private void ShuffleItems()
        {
            var currentIndex = GetPotentialDragIndex();
            
            // iterate over the items
            var offset = _dragImage.ActualHeight;
            for (var i = 0; i < _cards.Count; i++)
            {
                var item = _list.ItemContainerGenerator.ContainerFromIndex(i) as FrameworkElement;
                if (i <= currentIndex && i > _initialDragIndex)
                    OffsetItem(-offset, item);
                else if (i >= currentIndex && i < _initialDragIndex)
                    OffsetItem(offset, item);
                else
                    OffsetItem(0, item);
            }
        }

        private void OffsetItem(double offset, FrameworkElement element)
        {
            var targetLocation = element.Tag != null ? (double) element.Tag : 0;
            if (targetLocation == offset) return;

            var transform = element.GetVerticalOffset().Transform;
            transform.Animate(null, offset, TranslateTransform.YProperty, 500, 0);

            element.Tag = offset;
        }
    }
}
