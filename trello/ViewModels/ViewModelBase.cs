using System;
using System.Collections.Generic;
using System.Windows;
using Caliburn.Micro;

namespace trello.ViewModels
{
    public abstract class ViewModelBase : Screen
    {
        protected override void OnViewReady(object view)
        {
            if (!(view is DependencyObject))
                throw new InvalidOperationException("The view must be a DependencyObject for ViewModelBase to be " +
                                                    "use as its ViewModel.");

            Caliburn.Micro.Action.Invoke(this, "OnViewReady", (DependencyObject) view);
        }

        public virtual IEnumerable<IResult> OnViewReady()
        {
            yield break;
        }
    }
}