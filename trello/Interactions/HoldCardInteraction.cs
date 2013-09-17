using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using trello.Extensions;
using trello.ViewModels;
using trello.ViewModels.Cards;
using trello.Views;
using trello.Views.Cards;

namespace trello.Interactions
{
    public class HoldCardInteraction : CompositeInteractionManager
    {
        private readonly ItemsControl _context;
        private readonly DragImage _draggedImage;
        private readonly ScrollViewer _scrollViewer;

        private FrameworkElement _cardView;
        private Point _originalRelativePosition;

        public HoldCardInteraction(DragImage draggedImage, ItemsControl context, ScrollViewer scrollViewer)
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
                UnfadeCards(_context);

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
                var selected = (CardViewModel)_cardView.DataContext;

                var scrollOffset = new Point(_scrollViewer.HorizontalOffset, _scrollViewer.VerticalOffset);
                _originalRelativePosition = _cardView.GetRelativePositionIn(_context, scrollOffset);

                // Fade everything out
                //if (_context != null)
                //    _context.Animate(1.0, 0.7, UIElement.OpacityProperty, 300, 0);

                if (_context != null)
                    FadeCards(_context, selected);

                // Popout the selected card
                // PopoutCard(_cardView, _draggedImage, _originalRelativePosition, scrollOffset);

                // We can allow children to listen to events now
                EnableChildInteractions();
            }
        }

        private static void FadeCards(ItemsControl context, CardViewModel selected)
        {

            var containers = context.ItemsSource.OfType<CardViewModel>()
                                     .Select(vm => new
                                     {
                                         container = (FrameworkElement)context.ItemContainerGenerator.ContainerFromItem(vm),
                                         model = vm
                                     });
            foreach (var item in containers)
            {
                if (item.model != selected)
                    item.container.Animate(1.0, 0.5, UIElement.OpacityProperty, 800, 0);
                else
                    item.container.Opacity = 1.0;
            }
        }

        private static void UnfadeCards(ItemsControl context, Action completed = null)
        {
            foreach (var item in context.ItemsSource.OfType<CardViewModel>()
                                        .Select(vm => context.ItemContainerGenerator.ContainerFromItem(vm))
                                        .Cast<FrameworkElement>())
            {
                if (item != null)
                    item.Animate(null, 1.0, UIElement.OpacityProperty, 700, 0, completed: completed);
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
    }
}