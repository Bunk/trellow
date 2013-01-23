using System;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using trellow.api.Models;

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

        public AttachmentViewModel For(Attachment attachment)
        {
            if (attachment.Previews.Any())
            {
                var preview = attachment.Previews
                    .OrderBy(p => p.Width)
                    .ThenBy(p => p.Height)
                    .First();

                PreviewUri = new Uri(preview.Url, UriKind.Absolute);
                PreviewWidth = preview.Width;
                PreviewHeight = preview.Height;
            }

            Name = attachment.Name;
            Date = DateTime.SpecifyKind(attachment.Date, DateTimeKind.Utc).ToLocalTime();
            Bytes = attachment.Bytes;
            Uri = string.IsNullOrWhiteSpace(attachment.Url) ? null : new Uri(attachment.Url, UriKind.Absolute);

            var extension = Path.GetExtension(attachment.Url);
            if (extension != null)
                Extension = extension.Substring(1).ToUpperInvariant();

            return this;
        }
    }
}