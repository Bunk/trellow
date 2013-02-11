using System;

namespace trello.ViewModels
{
    public class ChangeCardNameViewModel : DialogViewModel
    {
        public ChangeCardNameViewModel(object root) : base(root)
        {
        }

        public string Name { get; set; }

        public Action<string> Accepted { get; set; }

        public void Accept()
        {
            Accepted(Name);
            TryClose();
        }
    }
}