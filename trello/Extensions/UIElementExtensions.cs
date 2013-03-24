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

        public static Point GetMidpoint(this Point point, Size size)
        {
            return new Point(point.X + size.Width / 2, point.Y + size.Height / 2);
        }
    }
}