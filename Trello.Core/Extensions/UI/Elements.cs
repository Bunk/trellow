using System.Windows;

namespace Trellow.UI
{
    public static class UiElementExtensions
    {
        public static Point GetRelativePositionIn(this UIElement element, UIElement other)
        {
            return element.TransformToVisual(other)
                .Transform(new Point(0, 0));
        }

        public static Point GetRelativePositionIn(this UIElement element, UIElement context, Point offset)
        {
            var point = element.GetRelativePositionIn(context);
            return new Point(point.X + offset.X, point.Y + offset.Y);
        }

        public static Point GetMidpoint(this Point point, Size size)
        {
            return new Point(point.X + size.Width / 2, point.Y + size.Height / 2);
        }

        public static Point Midpoint(this Rect rect)
        {
            return new Point(rect.Left + rect.Width/2, rect.Top + rect.Height/2);
        }

        public static bool ContainsInclusive(this Rect rect, Point point)
        {
            if (point.X >= rect.Left && (rect.Left + rect.Width) > point.X &&
                point.Y >= rect.Top && (rect.Top + rect.Height) > point.Y)
                return true;
            return false;
        }
    }
}