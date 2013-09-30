using System;
using Microsoft.Phone.Shell;

namespace Trellow.Services.UI
{
    public interface IBuildApplicationBars
    {
        IBuildApplicationBars Setup(Action<ApplicationBar> config);

        ApplicationBar Build();
    }

    public interface IBuildApplicationBarsWithDefaults : IBuildApplicationBars
    {
        IBuildApplicationBars Defaults();
    }

    public class ApplicationBarBuilder : IBuildApplicationBarsWithDefaults
    {
        private readonly ApplicationBar _applicationBar;
        private Action<ApplicationBar> _defaults;

        public ApplicationBarBuilder()
        {
            _applicationBar = new ApplicationBar();
        }

        public ApplicationBarBuilder WithDefaults(Action<ApplicationBar> defaults)
        {
            _defaults = defaults;
            return this;
        }

        public IBuildApplicationBars Setup(Action<ApplicationBar> config)
        {
            config(_applicationBar);
            return this;
        }

        public ApplicationBar Build()
        {
            return _applicationBar;
        }

        public IBuildApplicationBars Defaults()
        {
            if (_defaults != null)
                _defaults(_applicationBar);

            return this;
        }
    }
}