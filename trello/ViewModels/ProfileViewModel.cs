using Caliburn.Micro;
using JetBrains.Annotations;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using TrelloNet;
using trello.Extensions;
using trellow.api;

namespace trello.ViewModels
{
    public class ProfileViewModel : ViewModelBase, IConfigureTheAppBar
    {
        private readonly ITrello _api;
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
                                INavigationService navigation,
                                IEventAggregator eventAggregator) : base(settings, navigation)
        {
            _api = api;
            _eventAggregator = eventAggregator;
        }

        [UsedImplicitly]
        public void EmailUser()
        {
            _eventAggregator.RequestTask<EmailComposeTask>(task => { task.To = Email; });
        }

        protected override async void OnInitialize()
        {
            var profile = await _api.Members.Me();
            InitializeWith(profile);
        }

        private void InitializeWith(Member profile)
        {
            if (profile == null)
                return;

            Username = profile.Username;
            FullName = profile.FullName;
            Email = profile.Email;
            ImageUri = profile.AvatarHash.ToAvatarUrl(AvatarSize.Portrait);
            Bio = profile.Bio;
        }

        public ApplicationBar Configure(ApplicationBar existing)
        {
            return existing;
        }
    }
}