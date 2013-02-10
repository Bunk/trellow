using System;
using Caliburn.Micro;

namespace trello.ViewModels
{
    public class ChangeCardNameViewModel : Screen
    {
        public string Name { get; set; }

        public Action<string> Accepted { get; set; }

        public void Accept()
        {
            Accepted(Name);
            TryClose();
        }
    }
}