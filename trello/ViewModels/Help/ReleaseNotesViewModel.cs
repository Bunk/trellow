using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Strilanc.Value;
using trello.Assets;
using trello.Extensions;
using trello.Services;

namespace trello.ViewModels.Help
{
    public class ReleaseNotesViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private const string Filename = "ReleaseNotes.json";

        private string _maximumVersion;
        private string _minimumVersion;

        [UsedImplicitly]
        public string MaximumVersion
        {
            get { return _maximumVersion; }
            set
            {
                if (Equals(value, _maximumVersion)) return;
                _maximumVersion = value;
                NotifyOfPropertyChange(() => MaximumVersion);
            }
        }

        [UsedImplicitly]
        public string MinimumVersion
        {
            get { return _minimumVersion; }
            set
            {
                if (Equals(value, _minimumVersion)) return;
                _minimumVersion = value;
                NotifyOfPropertyChange(() => MinimumVersion);
            }
        }

        [UsedImplicitly]
        public IObservableCollection<ReleaseNoteViewModel> Notes { get; set; }

        public ReleaseNotesViewModel(IApplicationBar applicationBar, INavigationService navigationService)
            : base(applicationBar)
        {
            _navigationService = navigationService;

            Notes = new BindableCollection<ReleaseNoteViewModel>();
        }

        protected override async void OnInitialize()
        {
            // Could probably cache this since it won't change past app resets?
            var data = ReadListFromFile<ReleaseNoteViewModel>(Filename);
            var min = TryParseVersion(MinimumVersion);
            var max = TryParseVersion(MaximumVersion);
            var notes = (await data)
                .Where(note => min.Match(version => note.Version > version, () => true))
                .Where(note => max.Match(version => note.Version <= version, () => true));

            Notes.Clear();
            Notes.AddRange(notes);
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            // Only show these options when we're coming here from the login / splash screen
            if (_navigationService.RedirectedFrom<SplashViewModel>())
                AppBar.UpdateWith(
                    config =>
                    config.Setup(bar => bar.AddButton("ok", new AssetUri("Icons/dark/appbar.check.rest.png"), Accept)));
        }

        protected override void OnDeactivate(bool close)
        {
            AppVersion.Current.IfHasValueThenDo(AppVersion.UpdatePreviousVersion);

            base.OnDeactivate(close);
        }

        private void Accept()
        {
            _navigationService.GoBack();
        }

        private May<Version> TryParseVersion(string version)
        {
            if (version == null)
                return May<Version>.NoValue;

            Version parsed;
            return Version.TryParse(version, out parsed) ? parsed : May<Version>.NoValue;
        }

        private static async Task<IList<T>> ReadListFromFile<T>(string filename)
        {
            if (!File.Exists(filename))
                return new List<T>();

            using (var stream = File.OpenText(filename))
            {
                var content = await stream.ReadToEndAsync();
                return Deserialize<List<T>>(content);
            }
        }

        private static T Deserialize<T>(string data)
        {
            return JsonConvert.DeserializeObject<T>(data, new VersionConverter());
        }
    }
}