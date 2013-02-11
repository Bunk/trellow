using System;

namespace trello.ViewModels
{
    public class ChangeCardDueViewModel : DialogViewModel
    {
        public DateTime? Date { get; set; }

        public Action<DateTime> Accepted { get; set; }

        public Action Removed { get; set; }

        public ChangeCardDueViewModel(object root) : base(root) { }

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