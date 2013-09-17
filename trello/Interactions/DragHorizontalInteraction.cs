using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Caliburn.Micro;
using LinqToVisualTree;
using trello.Extensions;
using trello.Services.Handlers;
using trello.Services.Messages;
using trello.ViewModels;
using trello.ViewModels.Cards;
using Action = System.Action;

namespace trello.Interactions
{
    public class DragHorizontalInteraction : InteractionBase
    {
        private const double MinimumDragDistance = 1.0;
        private const double FlickVelocity = 2000.0;

        private readonly IEventAggregator _eventAggregator;
        private readonly string _previousListId;
        private readonly string _nextListId;
        private readonly ItemsControl _itemsControl;
        private FrameworkElement _dragCues;

        private readonly BindableCollection<CardViewModel> _cardsModel;

        public DragHorizontalInteraction(ItemsControl itemsControl,
                                         IEventAggregator eventAggregator,
                                         string previousListId,
                                         string nextListId)
        {
            _itemsControl = itemsControl;
            _eventAggregator = eventAggregator;
            _previousListId = previousListId;
            _nextListId = nextListId;

            _cardsModel = (BindableCollection<CardViewModel>) itemsControl.ItemsSource;
        }

        public override void AddElement(FrameworkElement element)
        {
            element.ManipulationDelta += ManipulationDelta;
            element.ManipulationCompleted += ManipulationCompleted;
        }

        private void ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            if (!IsEnabled)
                return;

            if (!IsActive && !ShouldActivate(e))
                return;

            var element = sender as FrameworkElement;
            if (element == null)
                return;

            e.Handled = true;

            if (!IsActive)
            {
                IsActive = true;

                // Initialize
                element.SetHorizontalOffset(0.0);

                var container = _itemsControl.ItemContainerGenerator.ContainerFromItem(element.DataContext);
                _dragCues = container.Descendants()
                                     .OfType<FrameworkElement>()
                                     .SingleOrDefault(fe => fe.Name == "MoveCues");
            }
            else
            {
                var offset = element.GetHorizontalOffset().Value + e.DeltaManipulation.Translation.X;
                UpdateDragCues(element, e.CumulativeManipulation.Translation.X, offset);
                element.SetHorizontalOffset(offset);
            }
        }

        private void ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            if (!IsActive || !IsEnabled)
                return;

            try
            {
                var element = sender as FrameworkElement;
                if (element == null)
                    return;

                if (HasPassedThresholds(element.ActualWidth,
                                        e.TotalManipulation.Translation.X,
                                        e.FinalVelocities.LinearVelocity.X))
                {
                    var movedLeft = e.TotalManipulation.Translation.X < 0.0;

                    // Perform the actions
                    var item = (CardViewModel) ((FrameworkElement) sender).DataContext;
                    var evt = new CardMovedToList
                    {
                        Card = item.OriginalCard,
                        SourceListId = item.ListId,
                        DestinationListId = movedLeft ? _previousListId : _nextListId
                    };

                    // Animate accordingly
                    AnimateMove(element, movedLeft, () =>
                    {
                        _eventAggregator.Publish(evt);
                        _cardsModel.Remove(item);
                        _cardsModel.Refresh();

                        Complete();
                    });
                }
                else
                {
                    // Cancel action
                    AnimateCancel(element, () =>
                    {
                        UpdateDragCues(element, 0, 0);
                        Complete();
                    });
                }
            }
            finally
            {
                IsActive = false;
                Complete();
            }
        }

        private static void AnimateCancel(FrameworkElement element, Action finished)
        {
            var translate = element.GetHorizontalOffset().Transform;
            translate.Animate(translate.TranslateX, 0, CompositeTransform.TranslateXProperty, 600, 0, new BounceEase
            {
                Bounces = 2,
                Bounciness = 10
            }, finished);
        }

        private void AnimateMove(FrameworkElement element, bool movedLeft, Action performDelete)
        {
            UpdateDragCues(element, 0, 0);

            var translation = element.ActualWidth + 50;
            if (movedLeft)
                translation = -translation;

            var translate = element.GetHorizontalOffset().Transform;
            translate.Animate(translate.TranslateX, translation, CompositeTransform.TranslateXProperty, 600, 0,
                              new SineEase {EasingMode = EasingMode.EaseOut}, () =>
                              {
                                  var deletedItem = element.DataContext as CardViewModel;

                                  var offset = -element.ActualHeight;

                                  // Animate this stuff to make it a litte prettier
                                  var itemsInView = _itemsControl.GetItemsInView().ToList();
                                  var lastItem = itemsInView.Last();
                                  var startTime = 0;
                                  var deletedItemIndex = itemsInView.Select(i => i.DataContext).ToList()
                                                                    .IndexOf(deletedItem);

                                  foreach (var item in itemsInView.Skip(deletedItemIndex))
                                  {
                                      var transform = item.GetVerticalOffset().Transform;
                                      transform.Animate(0, offset, CompositeTransform.TranslateYProperty, 200, startTime,
                                                        null, item == lastItem ? performDelete : null);

                                      startTime += 10;
                                  }
                              });
        }

        private static bool ShouldActivate(ManipulationDeltaEventArgs e)
        {
            return Math.Abs(e.CumulativeManipulation.Translation.X) >= MinimumDragDistance;
        }

        private static bool HasPassedThresholds(double elementWidth, double totalHorizontalTranslation,
                                                double totalLinearVelocity)
        {
            var midpoint = Math.Abs(totalHorizontalTranslation) > elementWidth*.4;
            var velocity = Math.Abs(totalLinearVelocity) > FlickVelocity;
            return midpoint || velocity;
        }

        private void UpdateDragCues(FrameworkElement fe, double totalHorizontalOffset, double currentHorizontalOffset)
        {
            if (HasPassedThresholds(fe.ActualWidth, totalHorizontalOffset, 0))
            {
                // Show the green arrows now
                UpdateOpacity<TextBlock>(_dragCues, 0.0);
                UpdateOpacity<Canvas>(_dragCues, 1.0);
            }
            else
            {
                var opacity = CalcDragCueOpacity(currentHorizontalOffset);
                UpdateOpacity<TextBlock>(_dragCues, opacity);
                UpdateOpacity<Canvas>(_dragCues, 0.0);
            }
        }

        private static double CalcDragCueOpacity(double offset)
        {
            offset = Math.Abs(offset);
            if (offset < 50)
                return 0;

            offset -= 50;
            var opacity = offset/100;

            // normalize the values between 0 and 1
            opacity = Math.Max(Math.Min(opacity, 1), 0);

            return opacity;
        }

        private static void UpdateOpacity<T>(DependencyObject fe, double opacity)
        {
            foreach (FrameworkElement el in fe.Descendants<T>())
            {
                el.Opacity = opacity;
            }
        }
    }
}