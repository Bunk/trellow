using System;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using TrelloNet;
using trellow.api.Checklists;

namespace trello.Services.Handlers
{
    public class CardDetailCommandHandler : IHandle<CardNameChanged>,
                                            IHandle<CardDescriptionChanged>,
                                            IHandle<CheckItemChanged>,
                                            IHandle<CardDueDateChanged>,
                                            IHandle<CardLabelAdded>,
                                            IHandle<CardLabelRemoved>,
                                            IHandle<CardMemberAdded>,
                                            IHandle<CardMemberRemoved>
    {
        private readonly ITrello _api;
        private readonly IProgressService _progress;

        public CardDetailCommandHandler(IEventAggregator eventAggregator, ITrello api, IProgressService progress)
        {
            _api = api;
            _progress = progress;

            eventAggregator.Subscribe(this);
        }

        private class ProgressService : IDisposable
        {
            private readonly IProgressService _progress;

            public ProgressService(IProgressService progress)
            {
                _progress = progress;
                _progress.Show("Updating...");
            }

            public void Dispose()
            {
                _progress.Hide();
            }
        }

        private async void Handle(Func<ITrello, Task> handler)
        {
            using (new ProgressService(_progress))
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
            Handle(api => api.Async.Cards.ChangeDescription(new CardId(message.CardId), message.Description));
        }

        public void Handle(CardDueDateChanged message)
        {
            Handle(api => api.Async.Cards.ChangeDueDate(new CardId(message.CardId), message.DueDate));
        }

        public void Handle(CardLabelAdded message)
        {
            Handle(api => api.Async.Cards.AddLabel(new CardId(message.CardId), message.Color));
        }

        public void Handle(CardLabelRemoved message)
        {
            Handle(api => api.Async.Cards.RemoveLabel(new CardId(message.CardId), message.Color));
        }

        public void Handle(CardNameChanged message)
        {
            Handle(api => api.Async.Cards.ChangeName(new CardId(message.CardId), message.Name));
        }

        public void Handle(CheckItemChanged message)
        {
            Handle(api => api.Async.Cards.ChangeCheckItemState(new CardId(message.CardId),
                                                               new ChecklistId(message.ChecklistId),
                                                               new CheckItemId(message.CheckItemId),
                                                               message.Value));
        }

        public void Handle(CardMemberAdded message)
        {
            Handle(api => api.Async.Cards.AddMember(new CardId(message.CardId), new MemberId(message.MemberId)));
        }

        public void Handle(CardMemberRemoved message)
        {
            Handle(api => api.Async.Cards.RemoveMember(new CardId(message.CardId), new MemberId(message.MemberId)));
        }
    }
}