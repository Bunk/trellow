using System;
using System.Text.RegularExpressions;
using trellow.api.Models;

namespace trello.ViewModels.Activities
{
    public class ActivityViewModel
    {
        public string Id { get; set; }

        public MemberViewModel Member { get; set; }

        public DateTime Timestamp { get; set; }

        public ActivityType Type { get; set; }

        public ActivityData Data { get; set; }

        public static ActivityViewModel For(Activity activity)
        {
            var model = new ActivityViewModel();

            switch (activity.Type)
            {
                case ActivityType.CreateCard:
                    model = new SimpleActivityViewModel
                    {
                        Action = "added",
                        ActionName = activity.Data.Card.Name,
                        TargetName = activity.Data.List.Name
                    };
                    break;
                case ActivityType.AddAttachmentToCard:
                    model = new SimpleActivityViewModel
                    {
                        Action = "attached",
                        ActionName = activity.Data.Attachment.Name,
                        ActionUrl = activity.Data.Attachment.Url,
                        ActionImageUri = UpdateWithImageUri(activity.Data.Attachment.Url),
                        TargetName = activity.Data.Card.Name
                    };
                    break;
                case ActivityType.AddChecklistToCard:
                    model = new SimpleActivityViewModel
                    {
                        Action = "added",
                        ActionName = activity.Data.Checklist.Name,
                        TargetName = activity.Data.Card.Name
                    };
                    break;
                case ActivityType.CommentCard:
                    model = new CommentActivityViewModel
                    {
                        Text = activity.Data.Text,
                        LastEditedDate = activity.Data.DateLastEdited,
                        TargetName = activity.Data.Card.Name
                    };
                    break;
            }

            model.Id = activity.Id;
            model.Member = new MemberViewModel(activity.MemberCreator);
            model.Timestamp = DateTime.SpecifyKind(activity.Date, DateTimeKind.Utc).ToLocalTime();
            model.Type = activity.Type;
            model.Data = activity.Data;

            return model;
        }

        private static readonly Regex ImageExtensionRegex = new Regex(@"([^\s])+(\.(?i)(jpg|png|gif|bmp))$",
                                                                      RegexOptions.Compiled);

        private static Uri UpdateWithImageUri(string url)
        {
            return ImageExtensionRegex.IsMatch(url) ? new Uri(url, UriKind.Absolute) : null;
        }
    }

    public class SimpleActivityViewModel : ActivityViewModel
    {
        public string Action { get; set; }

        public string ActionName { get; set; }

        public string ActionUrl { get; set; }

        public Uri ActionImageUri { get; set; }

        public string TargetName { get; set; }
    }

    public class CommentActivityViewModel : ActivityViewModel
    {
        public string Text { get; set; }

        public string TargetName { get; set; }

        public DateTime? LastEditedDate { get; set; }
    }
}