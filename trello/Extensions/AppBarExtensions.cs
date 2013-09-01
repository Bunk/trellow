using System;
using Microsoft.Phone.Shell;

namespace trello.Extensions
{
    public static class AppBarExtensions
    {
        public static void AddMenuItem(this ApplicationBar bar, string text, EventHandler handler)
        {
            // todo: make this memory leak safe
            var delete = new ApplicationBarMenuItem(text);
            delete.Click += handler;
            bar.MenuItems.Add(delete);
        }

        public static void AddButton(this ApplicationBar bar, string text, Uri image, EventHandler handler)
        {
            // todo: make this memory leak safe
            var archive = new ApplicationBarIconButton {Text = text, IconUri = image};
            archive.Click += handler;
            bar.Buttons.Add(archive);
        }
    }
}