using System;
using Microsoft.Phone.Shell;

namespace trello.Extensions
{
    public static class AppBarExtensions
    {
        public static ApplicationBar AddMenuItem(this ApplicationBar bar, string text, Action handler)
        {
            return AddMenuItem(bar, text, (sender, args) => handler());
        }

        public static ApplicationBar AddMenuItem(this ApplicationBar bar, string text, EventHandler handler)
        {
            // todo: make this memory leak safe
            var delete = new ApplicationBarMenuItem(text);
            delete.Click += handler;
            bar.MenuItems.Add(delete);

            return bar;
        }

        public static ApplicationBar AddButton(this ApplicationBar bar, string text, Uri image, Action handler)
        {
            return AddButton(bar, text, image, (sender, args) => handler());
        }

        public static ApplicationBar AddButton(this ApplicationBar bar, string text, Uri image, EventHandler handler)
        {
            // todo: make this memory leak safe
            var archive = new ApplicationBarIconButton {Text = text, IconUri = image};
            archive.Click += handler;
            bar.Buttons.Add(archive);

            return bar;
        }
    }
}