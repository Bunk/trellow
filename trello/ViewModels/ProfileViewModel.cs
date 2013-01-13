using Caliburn.Micro;
using JetBrains.Annotations;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using trello.Services;
using trello.Services.Data;
using trello.Services.Models;

namespace trello.ViewModels
{
    public class ProfileViewModel : ViewModelBase, IConfigureTheAppBar
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IProfileService _profileService;
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

        public ProfileViewModel(ITrelloSettings settings,
                                INavigationService navigation,
                                IEventAggregator eventAggregator,
                                IProfileService profileService) : base(settings, navigation)
        {
            _eventAggregator = eventAggregator;
            _profileService = profileService;
        }

        [UsedImplicitly]
        public void EmailUser()
        {
            _eventAggregator.RequestTask<EmailComposeTask>(task => { task.To = Email; });
        }

        protected override async void OnInitialize()
        {
            var profile = await _profileService.Mine();
            InitializeWith(profile);
        }

        protected override async void OnActivate()
        {
            var profile = await _profileService.Mine();
            InitializeWith(profile);
        }

        private void InitializeWith(Profile profile)
        {
            const string profileImageFormat =
                "https://trello-avatars.s3.amazonaws.com/{0}/{1}.png";

            Username = "@" + profile.Username;
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