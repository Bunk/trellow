using System;
using System.IO;
using System.Linq;
using System.Windows;
using JetBrains.Annotations;
using TrelloNet;
using Windows.System;
using trello.Extensions;

namespace trello.ViewModels
{
    [UsedImplicitly]
    public class AttachmentViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public DateTime Date { get; set; }

        public int Bytes { get; set; }

        public Uri Uri { get; set; }

        public Uri PreviewUri { get; set; }

        public int PreviewWidth { get; set; }

        public int PreviewHeight { get; set; }

        public string Extension { get; set; }

        public bool IsCover { get; set; }

        public async void Launch()
        {
            var success = await Launcher.LaunchUriAsync(Uri);
            if (!success)
                MessageBox.Show(
                    "The attachment could not be opened.  Make sure you're still connected to the internet.",
                    "Connection Problem", MessageBoxButton.OK);
        }

        public AttachmentViewModel InitializeWith(Card.Attachment att)
        {
            if (att.Previews.Any())
            {
                var preview = att.Previews
                    .OrderBy(p => p.Width)
                    .ThenBy(p => p.Height)
                    .First();

                PreviewUri = preview.Url.ToUri();
                PreviewWidth = preview.Width;
                PreviewHeight = preview.Height;
            }

            Name = att.Name;
            Date = att.Date.ToLocalTime();
            Bytes = att.Bytes;
            Uri = att.Url.ToUri();

            var extension = Path.GetExtension(att.Url);
            if (extension != null)
                Extension = extension.Substring(1).ToUpperInvariant();

            return this;
        }
    }
}