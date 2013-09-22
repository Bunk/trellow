using System;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using Microsoft.Phone.Shell;
using Strilanc.Value;
using trello.Extensions;
using IApplicationBar = trello.Services.IApplicationBar;

namespace trello.ViewModels
{
    public abstract class DialogViewModel : Screen, INeedApplicationBar<DialogViewModel>
    {
        private readonly INavigationService _navigationService;

        private May<UIElement> _root = May<UIElement>.NoValue;

        private May<IDisposable> _scope = May<IDisposable>.NoValue;

        private May<IApplicationBar> _applicationBar = May<IApplicationBar>.NoValue;

        protected DialogViewModel(object root)
        {
            _root = root.MayCast<UIElement>();
            _scope = May<IDisposable>.NoValue;

            _navigationService = IoC.Get<INavigationService>();
        }

        protected override void OnActivate()
        {
            // disable underlying event handling
            _root.IfHasValueThenDo(ui => ui.IsHitTestVisible = false);
        }

        protected override void OnDeactivate(bool close)
        {
            //re-enable underlying event handling
            _root.IfHasValueThenDo(ui => ui.IsHitTestVisible = true);

            // reset the application bar
            _scope.IfHasValueThenDo(scope => scope.Dispose());
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            // undo what the window manager does... bastards
            _navigationService.CurrentContent.MayCast<PhoneApplicationPage>()
                              .IfHasValueThenDo(page =>
                              {
                                  if (page.ApplicationBar == null)
                                      return;

                                  // note: this will always enable buttons, so doesn't handle the general cases
                                  // where you really want them disabled
                                  var buttons = page.ApplicationBar.Buttons.Cast<IApplicationBarIconButton>();
                                  foreach (var button in buttons)
                                      button.IsEnabled = true;
                              });
        }

        public DialogViewModel Bind(IApplicationBar applicationBar)
        {
            _applicationBar = applicationBar.Maybe();
            return this;
        }

        protected void UpdateApplicationBar(Action<ApplicationBar> builder)
        {
            _applicationBar.IfHasValueThenDo(bar =>
            {
                var previous = bar.Instance;
                _scope = Disposable
                    .Create(() => bar.Update(previous))
                    .Maybe();

                bar.UpdateWith(config => config.Setup(builder));
            });
        }
    }
}