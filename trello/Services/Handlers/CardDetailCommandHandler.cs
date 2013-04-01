using System;
using System.Threading.Tasks;
using System.Windows;
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
            Handle(api => api.Cards.ChangeDescription(new CardId(message.CardId), message.Description));
        }

        public void Handle(CardDueDateChanged message)
        {
            Handle(api => api.Cards.ChangeDueDate(new CardId(message.CardId), message.DueDate));
        }

        public void Handle(CardLabelAdded message)
        {
            Handle(api => api.Cards.AddLabel(new CardId(message.CardId), message.Color));
        }

        public void Handle(CardLabelRemoved message)
        {
            Handle(api => api.Cards.RemoveLabel(new CardId(message.CardId), message.Color));
        }

        public void Handle(CardNameChanged message)
        {
            Handle(api => api.Cards.ChangeName(new CardId(message.CardId), message.Name));
        }

        public void Handle(CheckItemChanged message)
        {
            Handle(api => api.Cards.ChangeCheckItemState(new CardId(message.CardId),
                                                         new ChecklistId(message.ChecklistId),
                                                         new CheckItemId(message.CheckItemId),
                                                         message.Value));
        }

        public void Handle(CardMemberAdded message)
        {
            Handle(api => api.Cards.AddMember(new CardId(message.CardId), new MemberId(message.MemberId)));
        }

        public void Handle(CardMemberRemoved message)
        {
            Handle(api => api.Cards.RemoveMember(new CardId(message.CardId), new MemberId(message.MemberId)));
        }

        public void Handle(CardCommented message)
        {
            Handle(api => api.Cards.AddComment(new CardId(message.CardId), message.Text));
        }

        public void Handle(CardDeleted message)
        {
            Handle(api => api.Cards.Delete(new CardId(message.CardId)));
        }

        public void Handle(CardCreationRequested message)
        {
            Handle(async api =>
            {
                var created = await api.Cards.Add(new NewCard(message.Name, new ListId(message.ListId)));
                _events.Publish(new CardCreated {Card = created});
            });
        }

        public void Handle(CheckItemCreationRequested message)
        {
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
            Handle(api => api.Checklists.RemoveCheckItem(new ChecklistId(message.ChecklistId), message.CheckItemId));
        }

        public void Handle(ChecklistCreationRequested message)
        {
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
            Handle(api => api.Cards.RemoveChecklist(new CardId(message.CardId), new ChecklistId(message.ChecklistId)));
        }

        public void Handle(ChecklistNameChanged message)
        {
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
    }
}