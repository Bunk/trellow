using System.Windows;

namespace trello.Extensions
{
    public static class UiElementExtensions
    {
        public static Point GetRelativePositionIn(this UIElement element, UIElement other)
        {
            return element.TransformToVisual(other)
                .Transform(new Point(0, 0));
        }
    }
}