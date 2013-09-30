using Caliburn.Micro;
using Trellow.Diagnostics;
using Trellow.Events;
using Trellow.Services.UI;
using trellow.api;
using trellow.api.Boards;
using trellow.api.Cards;
using trellow.api.Checklists;

namespace Trellow.Handlers.Cards
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
            Analytics.TagEvent("Create_Checklist_Item");
            Handle(async api =>
            {
                var created = await api.Checklists.AddCheckItem(new ChecklistId(message.ChecklistId), message.Name);
                Analytics.TagEvent("Created_Checklist_Item");

                Events.Publish(new CheckItemCreated
                {
                    ChecklistId = message.ChecklistId,
                    CheckItem = created
                });
            });
        }

        public void Handle(CheckItemRemoved message)
        {
            Analytics.TagEvent("Removed_Checklist_Item");
            Handle(api => api.Checklists.RemoveCheckItem(new ChecklistId(message.ChecklistId), message.CheckItemId));
        }

        public void Handle(CheckItemChanged message)
        {
            //question: too much data?
//            if (message.Value)
//                Analytics.TagEvent("Checked_Checklist_Item");

            Handle(api => api.Cards.ChangeCheckItemState(new CardId(message.CardId),
                                                         new ChecklistId(message.ChecklistId),
                                                         new CheckItemId(message.CheckItemId),
                                                         message.Value));
        }

        public void Handle(ChecklistCreationRequested message)
        {
            Analytics.TagEvent("Create_Checklist");
            Handle(async api =>
            {
                var created = await api.Checklists.Add(message.Name, new BoardId(message.BoardId));
                await api.Cards.AddChecklist(new CardId(message.CardId), created);
                Analytics.TagEvent("Created_Checklist");

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
            Analytics.TagEvent("Removed_Checklist");
            Handle(api => api.Cards.RemoveChecklist(new CardId(message.CardId), new ChecklistId(message.ChecklistId)));
        }

        public void Handle(ChecklistNameChanged message)
        {
            Analytics.TagEvent("Renamed_Checklist");
            Handle(api => api.Checklists.ChangeName(new ChecklistId(message.ChecklistId), message.Name));
        }
    }
}