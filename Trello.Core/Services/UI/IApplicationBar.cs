using System;
using System.ComponentModel;
using Microsoft.Phone.Shell;

namespace Trellow.Services.UI
{
    public interface IApplicationBar : INotifyPropertyChanged
    {
        ApplicationBar Instance { get; }

        void Update(ApplicationBar applicationBar);

        void UpdateWith(Action<IBuildApplicationBarsWithDefaults> action);
    }
}
