using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using trello.Extensions;
using trello.Views;
using trello.Views.Cards;

namespace trello.Interactions
{
    public class HoldCardInteraction : CompositeInteractionManager
    {
        private readonly UIElement _context;
        private readonly DragImage _draggedImage;
        private readonly ScrollViewer _scrollViewer;

        private FrameworkElement _cardView;
        private Point _originalRelativePosition;

        public HoldCardInteraction(DragImage draggedImage, UIElement context, ScrollViewer scrollViewer)
        {
            _draggedImage = draggedImage;
            _context = context;
            _scrollViewer = scrollViewer;

            IsEnabled = true;
        }

        public override void AddElement(FrameworkElement element)
        {
            element.Hold += HoldGesture;
            element.ManipulationCompleted += HoldCompleted;

            EachChild(i => i.AddElement(element));
        }

        protected override void FinalizeInteraction()
        {
            // fade in the list
            if (_context != null)
                _context.Animate(null, 1.0, UIElement.OpacityProperty, 200, 0);

            // reshow the hidden item
            if (_cardView != null)
                _cardView.Opacity = 1.0;

            // fade out the dragged image
            if (_draggedImage != null)
                _draggedImage.Animate(null, 0.0, UIElement.OpacityProperty, 700, 0,
                                      completed: () => { _draggedImage.Visibility = Visibility.Collapsed; });

            // The composite is complete, so we no longer want children listening (shhh)
            DisableChildInteractions();
        }

        private void HoldGesture(object sender, GestureEventArgs e)
        {
            if (!IsEnabled)
                return;

            IsActive = true;

            _cardView = sender as CardView;
            if (_cardView == null)
            {
                IsActive = false;
            }
            else
            {
                var scrollOffset = new Point(_scrollViewer.HorizontalOffset, _scrollViewer.VerticalOffset);
                _originalRelativePosition = _cardView.GetRelativePositionIn(_context, scrollOffset);

                // Fade everything out
                if (_context != null)
                    _context.Animate(1.0, 0.7, UIElement.OpacityProperty, 300, 0);

                // Popout the selected card
                PopoutCard(_cardView, _draggedImage, _originalRelativePosition, scrollOffset);

                // We can allow children to listen to events now
                EnableChildInteractions();
            }
        }

        private void HoldCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            if (!IsActive)
                return;

            // Defer to children if any are active, otherwise we need to revert ourselves
            if (!AnyChildrenActive)
                FinalizeInteraction();

            IsActive = false;
        }

        private static void PopoutCard(UIElement element, DragImage image, Point relativePosition, Point scrollPosition)
        {
            // Copy the selected card into a bitmap for use as the movement target
            var bitmap = new WriteableBitmap(element, null);
            image.Image.Source = bitmap;
            image.Visibility = Visibility.Visible;
            image.Opacity = 1.0;
            image.SetRotation(3);

            // this needs to be relative to the scrolled position
            image.SetVerticalOffset(relativePosition.Y - scrollPosition.Y);
            image.SetHorizontalOffset(relativePosition.X - scrollPosition.Y);

            // hide the underlying item
            element.Opacity = 0.0;
        }
    }
}