using System;
using Caliburn.Micro;

namespace trello.ViewModels
{
    public class ChangeCardDueViewModel : Screen
    {
        public DateTime? Date { get; set; }

        public Action<DateTime> Accepted { get; set; }

        public System.Action Removed { get; set; }

        public void Confirm()
        {
            if (Date != null)
            {
                Accepted(Date.Value);
            }

            TryClose();
        }

        public void Remove()
        {
            Removed();

            TryClose();
        }
    }
}