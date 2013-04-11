using System;
using System.Threading.Tasks;
using System.Windows;
using BugSense;
using Caliburn.Micro;
using trellow.api;
using trellow.api.Boards;
using trellow.api.Cards;
using trellow.api.Checklists;
using trellow.api.Lists;
using trellow.api.Members;

namespace trello.Services.Handlers
{
    public class CardDetailCommandHandler : IHandle<CardNameChanged>,
                                            IHandle<CardDescriptionChanged>,
                                            IHandle<CheckItemChanged>,
                                            IHandle<CardDueDateChanged>,
                                            IHandle<CardLabelAdded>,
                                            IHandle<CardLabelRemoved>,
                                            IHandle<CardMemberAdded>,
                                            IHandle<CardMemberRemoved>,
                                            IHandle<CardCommented>,
                                            IHandle<CardDeleted>,
                                            IHandle<CardArchived>,
                                            IHandle<CardCreationRequested>,
                                            IHandle<CheckItemCreationRequested>,
                                            IHandle<CheckItemRemoved>,
                                            IHandle<ChecklistCreationRequested>,
                                            IHandle<ChecklistRemoved>,
                                            IHandle<ChecklistNameChanged>,
                                            IHandle<CardPriorityChanged>
    {
        private readonly IEventAggregator _events;
        private readonly ITrello _api;
        private readonly IProgressService _progress;

        public CardDetailCommandHandler(IEventAggregator events, ITrello api, IProgressService progress)
        {
            _events = events;
            _api = api;
            _progress = progress;

            events.Subscribe(this);
        }

        private async void Handle(Func<ITrello, Task> handler)
        {
            using (new ProgressScope(_progress, "Updating..."))
            {
                try
                {
                    await handler(_api);
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

        public void Handle(CardDescriptionChanged message)
        {
            BugSenseHandler.Instance.SendEvent("Update card description");
            Handle(api => api.Cards.ChangeDescription(new CardId(message.CardId), message.Description));
        }

        public void Handle(CardDueDateChanged message)
        {
            BugSenseHandler.Instance.SendEvent("Update card due date");
            Handle(api => api.Cards.ChangeDueDate(new CardId(message.CardId), message.DueDate));
        }

        public void Handle(CardLabelAdded message)
        {
            BugSenseHandler.Instance.SendEvent("Add label to card");
            Handle(api => api.Cards.AddLabel(new CardId(message.CardId), message.Color));
        }

        public void Handle(CardLabelRemoved message)
        {
            BugSenseHandler.Instance.SendEvent("Remove label from card");
            Handle(api => api.Cards.RemoveLabel(new CardId(message.CardId), message.Color));
        }

        public void Handle(CardNameChanged message)
        {
            BugSenseHandler.Instance.SendEvent("Change card name");
            Handle(api => api.Cards.ChangeName(new CardId(message.CardId), message.Name));
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

        public void Handle(CardMemberAdded message)
        {
            BugSenseHandler.Instance.SendEvent("Add member to card");
            Handle(api => api.Cards.AddMember(new CardId(message.CardId), new MemberId(message.MemberId)));
        }

        public void Handle(CardMemberRemoved message)
        {
            BugSenseHandler.Instance.SendEvent("Remove member from card");
            Handle(api => api.Cards.RemoveMember(new CardId(message.CardId), new MemberId(message.MemberId)));
        }

        public void Handle(CardCommented message)
        {
            BugSenseHandler.Instance.SendEvent("Comment on card");
            Handle(api => api.Cards.AddComment(new CardId(message.CardId), message.Text));
        }

        public void Handle(CardDeleted message)
        {
            BugSenseHandler.Instance.SendEvent("Delete card");
            Handle(api => api.Cards.Delete(new CardId(message.CardId)));
        }

        public void Handle(CardCreationRequested message)
        {
            BugSenseHandler.Instance.SendEvent("Create card");
            Handle(async api =>
            {
                var created = await api.Cards.Add(new NewCard(message.Name, new ListId(message.ListId)));
                _events.Publish(new CardCreated {Card = created});
            });
        }

        public void Handle(CheckItemCreationRequested message)
        {
            BugSenseHandler.Instance.SendEvent("Create checklist item");
            Handle(async api =>
            {
                var created = await api.Checklists.AddCheckItem(new ChecklistId(message.ChecklistId), message.Name);
                _events.Publish(new CheckItemCreated
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

        public void Handle(ChecklistCreationRequested message)
        {
            BugSenseHandler.Instance.SendEvent("Create checklist item");
            Handle(async api =>
            {
                var created = await api.Checklists.Add(message.Name, new BoardId(message.BoardId));
                await api.Cards.AddChecklist(new CardId(message.CardId), created);
                _events.Publish(new ChecklistCreated
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

        public void Handle(CardPriorityChanged message)
        {
            switch (message.Type)
            {
                case PositionType.Top:
                    Handle(api => api.Cards.ChangePos(new CardId(message.CardId), Position.Top));
                    break;
                case PositionType.Bottom:
                    Handle(api => api.Cards.ChangePos(new CardId(message.CardId), Position.Bottom));
                    break;
                case PositionType.Exact:
                    Handle(api => api.Cards.ChangePos(new CardId(message.CardId), message.Pos));
                    break;
            }
        }

        public void Handle(CardArchived message)
        {
            Handle(api => api.Cards.Archive(new CardId(message.CardId)));
        }
    }
}