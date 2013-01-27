using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using trellow.api;
using trellow.api.Data;
using trellow.api.OAuth;

namespace trello.livetile
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        private static readonly IOAuthClient OAuth;
        private static readonly ICardService CardService;

        /// <remarks>
        /// ScheduledAgent constructor, initializes the UnhandledException handler
        /// </remarks>
        static ScheduledAgent()
        {
            // Subscribe to the managed exception handler
            Deployment.Current.Dispatcher.BeginInvoke(
                delegate { Application.Current.UnhandledException += UnhandledException; });

            OAuth = new TrelloOAuthClient(new TrelloSettings());
            CardService = new CardService(new RequestProcessor(OAuth, null));
        }

        /// Code to execute on Unhandled Exceptions
        private static void UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                Debugger.Break();
            }
        }

        /// <summary>
        /// Agent that runs a scheduled task
        /// </summary>
        /// <param name="task">
        /// The invoked task
        /// </param>
        /// <remarks>
        /// This method is called when a periodic or resource intensive task is invoked
        /// </remarks>
        protected override async void OnInvoke(ScheduledTask task)
        {
            var tile = ShellTile.ActiveTiles.First();

            if (OAuth.ValidateAccessToken())
            {
                // We can access the api
                var cards = await CardService.Mine();
                tile.Update(new FlipTileData
                {
                    Count = cards.Count(),
                    BackTitle = cards[0].Name,
                    BackContent = cards[0].Desc,
                    WideBackContent = cards[0].Desc
                });
            }
            else
            {
                // We need to simply remove the data?
                tile.Update(new FlipTileData
                {
                    Count = 0,
                    BackTitle = "",
                    BackContent = "",
                    WideBackContent = ""
                });
            }

#if DEBUG
            ScheduledActionService.LaunchForTest(task.Name, TimeSpan.FromSeconds(10));
#endif

            NotifyComplete();
        }
    }
}