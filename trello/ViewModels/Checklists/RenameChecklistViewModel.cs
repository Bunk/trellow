using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using JetBrains.Annotations;
using trello.Services.Handlers;
using trello.Services.Messages;

namespace trello.ViewModels.Checklists
{
    [UsedImplicitly]
    public class RenameChecklistViewModel : DialogViewModel
    {
        private readonly IEventAggregator _events;
        private readonly string _checklistId;
        private string _name;

        [UsedImplicitly]
        public string Name
        {
            get { return _name; }
            set
            {
                if (value == _name) return;
                _name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }

        public RenameChecklistViewModel(object root, IEventAggregator events, string checklistId) : base(root)
        {
            _events = events;
            _checklistId = checklistId;
        }

        [UsedImplicitly]
        public void Accept()
        {
            _events.Publish(new ChecklistNameChanged
            {
                ChecklistId = _checklistId,
                Name = Name
            });
            TryClose();
        }
    }
}
