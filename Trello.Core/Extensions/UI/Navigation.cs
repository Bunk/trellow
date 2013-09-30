using System.Linq;
using Caliburn.Micro;

namespace Trellow.UI
{
    public static class NavigationExtensions
    {
        public static bool RedirectedFrom<T>(this INavigationService navigation)
        {
            if (!navigation.CanGoBack)
                return false;

            var previous = navigation.BackStack.First();
            if (previous.Source.IsAbsoluteUri)
                return false;

            var uriParts = navigation.UriFor<T>().BuildUri().OriginalString;
            return previous.Source.OriginalString.StartsWith(uriParts);
        }
    }
}