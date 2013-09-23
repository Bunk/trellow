using Caliburn.Micro;
using JetBrains.Annotations;
using Microsoft.Phone.Tasks;
using trello.ViewModels.Boards;

namespace trello.ViewModels
{
    [UsedImplicitly]
    public class AboutViewModel : Screen
    {
        private readonly INavigationService _navigation;

        [UsedImplicitly]
        public string Version { get; private set; }

        public AboutViewModel(INavigationService navigation)
        {
            _navigation = navigation;

            Version = ReadFromAppManifest();
        }

        private static string ReadFromAppManifest()
        {
            return AppVersion.Current.Match(version => version.ToString(4), () => null);
        }

        [UsedImplicitly]
        public void VisitTrellowBoard()
        {
            _navigation.UriFor<BoardViewModel>()
                .WithParam(x => x.Id, "50e349d3fd6ecef00d00409b")
                .Navigate();
        }

        [UsedImplicitly]
        public void RateAndReview()
        {
            var task = new MarketplaceReviewTask();
            task.Show();
        }

        [UsedImplicitly]
        public void SendFeedback()
        {
            var task = new EmailComposeTask
            {
                To = "jd.courtoy+trellow@gmail.com",
                Subject = "[Feedback] "
            };
            task.Show();
        }
    }
}