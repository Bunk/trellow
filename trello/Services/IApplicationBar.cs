using System;
using System.ComponentModel;
using Microsoft.Phone.Shell;

namespace trello.Services
{
    public interface IApplicationBar : INotifyPropertyChanged
    {
        ApplicationBar Instance { get; }

        void Update(ApplicationBar applicationBar);

        void UpdateWith(Action<IBuildApplicationBarsWithDefaults> action);
    }
}
