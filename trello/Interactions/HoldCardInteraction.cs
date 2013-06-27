using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using LinqToVisualTree;
using trello.Extensions;
using trello.Views;
using trello.Views.Cards;

namespace trello.Interactions
{
    public class HoldCardInteraction : InteractionManager
    {
        private readonly UIElement _context;
        private readonly DragImage _draggedImage;
        private readonly ScrollViewer _scrollViewer;

        private FrameworkElement _originalCard;
        private Point _originalRelativePosition;

        public HoldCardInteraction(DragImage draggedImage, UIElement context)
        {
            _draggedImage = draggedImage;
            _context = context;
            _scrollViewer = context.Descendants<ScrollViewer>().Cast<ScrollViewer>().SingleOrDefault();

            IsEnabled = true;
        }

        public override void AddElement(FrameworkElement element)
        {
            element.Hold += HoldGesture;

            EachChild(i => i.AddElement(element));
        }

        private void HoldGesture(object sender, GestureEventArgs e)
        {
            // copy the dragged element into an image to visually move it
            _originalCard = sender as CardView;
            if (_originalCard == null)
            {
                IsActive = false;
                return;
            }

            var scrollOffset = new Point(_scrollViewer.HorizontalOffset, _scrollViewer.VerticalOffset);
            _originalRelativePosition = _originalCard.GetRelativePositionIn(_context, scrollOffset);

            // Fade everything out
            if (_context != null)
                _context.Animate(1.0, 0.7, UIElement.OpacityProperty, 300, 0);

            // Popout the selected card
            PopoutCard(_originalCard, _draggedImage, _originalRelativePosition, scrollOffset);
        }

        protected override void ChildCompleted(object sender)
        {
            // fade in the list
            if (_context != null)
                _context.Animate(null, 1.0, UIElement.OpacityProperty, 200, 0);

            // reshow the hidden item
            if (_originalCard != null)
                _originalCard.Opacity = 1.0;

            // fade out the dragged image
            if (_draggedImage != null)
                _draggedImage.Animate(null, 0.0, UIElement.OpacityProperty, 700, 0,
                                      completed: () => { _draggedImage.Visibility = Visibility.Collapsed; });
        }

        private static void PopoutCard(UIElement element, DragImage image, Point relativePosition, Point scrollPosition)
        {
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
