using System;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using Trellow.Services.UI;
using trellow.api;

namespace Trellow.Handlers
{
    public abstract class AbstractHandler
    {
        protected readonly IEventAggregator Events;
        protected readonly ITrello Api;
        protected readonly IProgressService Progress;

        protected AbstractHandler(IEventAggregator events, ITrello api, IProgressService progress)
        {
            Api = api;
            Progress = progress;

            Events = events;
            Events.Subscribe(this);
        }

        protected virtual async void Handle(Func<ITrello, Task> handler)
        {
            using (new ProgressScope(Progress, "Updating..."))
            {
                try
                {
                    await handler(Api);
                }
                catch (TrelloUnauthorizedException)
                {
                    MessageBox.Show("You are unauthorized to complete that operation.");
                }
                catch (TrelloException)
                {
                    MessageBox.Show("There was an error in trying to complete that operation.");
                }
            }
        }
    }
}