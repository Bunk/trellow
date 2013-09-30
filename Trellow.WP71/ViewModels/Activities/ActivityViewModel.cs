using System;
using System.Text.RegularExpressions;
using trellow.api.Actions;
using Action = trellow.api.Actions.Action;

namespace Trellow.ViewModels.Activities
{
    public class ActivityViewModel
    {
        public string Id { get; set; }

        public MemberViewModel Member { get; set; }

        public DateTime Timestamp { get; set; }

        public static ActivityViewModel InitializeWith(Action activity)
        {
            var model = new ActivityViewModel();
            var act = activity as CommentCardAction;
            if (act != null)
            {
                model = new CommentActivityViewModel
                {
                    Text = act.Data.Text,
                    LastEditedDate = act.Data.DateLastEdited,
                    TargetName = act.Data.Card.Name
                };
            }

            model.Id = activity.Id;
            model.Member = new MemberViewModel(activity.MemberCreator);
            model.Timestamp = activity.Date.ToLocalTime();

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