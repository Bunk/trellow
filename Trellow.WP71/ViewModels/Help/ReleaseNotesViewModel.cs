using System;
using System.Linq;
using Caliburn.Micro;
using JetBrains.Annotations;
using Strilanc.Value;
using Trellow.Diagnostics;
using Trellow.Services.Data;
using Trellow.Services.UI;
using Trellow.UI;

namespace Trellow.ViewModels.Help
{
    public class ReleaseNotesViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IFileReader _fileReader;
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

        public ReleaseNotesViewModel(IApplicationBar applicationBar, INavigationService navigationService,
                                     IFileReader fileReader)
            : base(applicationBar)
        {
            _navigationService = navigationService;
            _fileReader = fileReader;

            Notes = new BindableCollection<ReleaseNoteViewModel>();
        }

        protected override void OnInitialize()
        {
            // Could probably cache this since it won't change past app resets?
            var data = _fileReader.ReadList<ReleaseNoteViewModel>(new Uri(Filename, UriKind.Relative));
            var min = MinimumVersion.MayParse<Version>(Version.TryParse);
            var max = MaximumVersion.MayParse<Version>(Version.TryParse);
            var notes = (data)
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
    }
}