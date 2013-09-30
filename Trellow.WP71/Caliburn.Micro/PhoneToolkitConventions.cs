using System.Windows.Controls;
using Microsoft.Phone.Controls;

namespace Caliburn.Micro
{
    public static class PhoneToolkitConventions
    {
        public static void Install()
        {
            ConventionManager.AddElementConvention<MenuItem>(ItemsControl.ItemsSourceProperty, "DataContext", "Click");
        }
    }
}