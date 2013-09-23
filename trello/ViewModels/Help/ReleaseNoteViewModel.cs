using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using trello.Services;

namespace trello.ViewModels.Help
{
    [UsedImplicitly]
    public class ReleaseNoteViewModel : ViewModelBase
    {
        [UsedImplicitly]
        public Version Version { get; set; }

        [UsedImplicitly]
        public List<Item> Items { get; set; }

        public ReleaseNoteViewModel(IApplicationBar applicationBar) : base(applicationBar)
        {
            Items = new List<Item>();
        }

        public enum ItemType
        {
            Feature,
            Bug
        }

        [UsedImplicitly]
        public class Item
        {
            [UsedImplicitly]
            public ItemType Type { get; set; }

            [UsedImplicitly]
            public string Note { get; set; }
        }
    }
}