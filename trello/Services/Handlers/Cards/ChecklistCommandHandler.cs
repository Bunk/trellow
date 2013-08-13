using BugSense;
using Caliburn.Micro;
using trello.Services.Messages;
using trellow.api;
using trellow.api.Boards;
using trellow.api.Cards;
using trellow.api.Checklists;

namespace trello.Services.Handlers.Cards
{
    public class ChecklistCommandHandler : AbstractHandler,
                                           IHandle<CheckItemCreationRequested>,
                                           IHandle<CheckItemRemoved>,
                                           IHandle<CheckItemChanged>,
                                           IHandle<ChecklistCreationRequested>,
                                           IHandle<ChecklistRemoved>,
                                           IHandle<ChecklistNameChanged>
    {
        public ChecklistCommandHandler(IEventAggregator events, ITrello api, IProgressService progress)
            : base(events, api, progress)
        {
        }

        public void Handle(CheckItemCreationRequested message)
        {
            BugSenseHandler.Instance.SendEvent("Create checklist item");
            Handle(async api =>
            {
                var created = await api.Checklists.AddCheckItem(new ChecklistId(message.ChecklistId), message.Name);
                Events.Publish(new CheckItemCreated
                {
                    ChecklistId = message.ChecklistId,
                    CheckItem = created
                });
            });
        }

        public void Handle(CheckItemRemoved message)
        {
            BugSenseHandler.Instance.SendEvent("Remove checklist item");
            Handle(api => api.Checklists.RemoveCheckItem(new ChecklistId(message.ChecklistId), message.CheckItemId));
        }

        public void Handle(CheckItemChanged message)
        {
            //note: too much data... 
            //BugSenseHandler.Instance.SendEvent("Toggle checklist item state");
            Handle(api => api.Cards.ChangeCheckItemState(new CardId(message.CardId),
                                                         new ChecklistId(message.ChecklistId),
                                                         new CheckItemId(message.CheckItemId),
                                                         message.Value));
        }

        public void Handle(ChecklistCreationRequested message)
        {
            BugSenseHandler.Instance.SendEvent("Create checklist item");
            Handle(async api =>
            {
                var created = await api.Checklists.Add(message.Name, new BoardId(message.BoardId));
                await api.Cards.AddChecklist(new CardId(message.CardId), created);
                Events.Publish(new ChecklistCreated
                {
                    CardId = message.CardId,
                    BoardId = message.BoardId,
                    ChecklistId = created.Id,
                    Name = created.Name
                });
            });
        }

        public void Handle(ChecklistRemoved message)
        {
            BugSenseHandler.Instance.SendEvent("Remove checklist from card");
            Handle(api => api.Cards.RemoveChecklist(new CardId(message.CardId), new ChecklistId(message.ChecklistId)));
        }

        public void Handle(ChecklistNameChanged message)
        {
            BugSenseHandler.Instance.SendEvent("Rename checklist");
            Handle(api => api.Checklists.ChangeName(new ChecklistId(message.ChecklistId), message.Name));
        }
    }
}