using System.Windows;
using Caliburn.Micro;
using JetBrains.Annotations;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using TrelloNet;
using trello.Services;
using trellow.api;

namespace trello.ViewModels
{
    public class ProfileViewModel : ViewModelBase, IConfigureTheAppBar
    {
        private readonly ITrello _api;
        private readonly IProgressService _progress;
        private readonly IEventAggregator _eventAggregator;
        private string _username;
        private string _fullName;
        private string _imageUri;
        private string _bio;
        private string _email;

        public string Username
        {
            get { return _username; }
            set
            {
                if (value == _username) return;
                _username = value;
                NotifyOfPropertyChange(() => Username);
            }
        }

        public string FullName
        {
            get { return _fullName; }
            set
            {
                if (value == _fullName) return;
                _fullName = value;
                NotifyOfPropertyChange(() => FullName);
            }
        }

        public string Email
        {
            get { return _email; }
            set
            {
                if (value == _email) return;
                _email = value;
                NotifyOfPropertyChange(() => Email);
            }
        }

        public string ImageUri
        {
            get { return _imageUri; }
            set
            {
                if (value == _imageUri) return;
                _imageUri = value;
                NotifyOfPropertyChange(() => ImageUri);
            }
        }

        public string Bio
        {
            get { return _bio; }
            set
            {
                if (value == _bio) return;
                _bio = value;
                NotifyOfPropertyChange(() => Bio);
            }
        }

        public ProfileViewModel(ITrello api,
                                ITrelloApiSettings settings,
                                IProgressService progress,
                                INavigationService navigation,
                                IEventAggregator eventAggregator) : base(settings, navigation)
        {
            _api = api;
            _progress = progress;
            _eventAggregator = eventAggregator;
        }

        [UsedImplicitly]
        public void EmailUser()
        {
            _eventAggregator.RequestTask<EmailComposeTask>(task => { task.To = Email; });
        }

        protected override async void OnInitialize()
        {
            _progress.Show("Loading...");
            try
            {
                var profile = await _api.Async.Members.Me();
                InitializeWith(profile);
            }
            catch
            {
                MessageBox.Show("Could not load your profile.  Please ensure " +
                                "that you have an active internet connection.");
            }
            _progress.Hide();
        }

        private void InitializeWith(Member profile)
        {
            const string profileImageFormat =
                "https://trello-avatars.s3.amazonaws.com/{0}/{1}.png";

            if (profile == null) 
                return;

            Username = profile.Username;
            FullName = profile.FullName;
            Email = profile.Email;
            ImageUri = string.Format(profileImageFormat, profile.AvatarHash, 170);
            Bio = profile.Bio;
        }

        public ApplicationBar ConfigureTheAppBar(ApplicationBar existing)
        {
            return existing;
        }
    }
}