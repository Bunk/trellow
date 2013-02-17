using System;
using Caliburn.Micro;
using TrelloNet;
using trellow.api.Checklists;

namespace trello.Services.Handlers
{
    public class CardDetailCommandHandler : IHandle<CardNameChanged>,
                                            IHandle<CardDescriptionChanged>,
                                            IHandle<CheckItemChanged>,
                                            IHandle<CardDueDateChanged>,
                                            IHandle<CardLabelsChanged>
    {
        private readonly ITrello _api;
        private readonly IProgressService _progress;

        public CardDetailCommandHandler(IEventAggregator eventAggregator, ITrello api, IProgressService progress)
        {
            _api = api;
            _progress = progress;

            eventAggregator.Subscribe(this);
        }

        public async void Handle(CardNameChanged message)
        {
            using (new ProgressService(_progress))
                await _api.Async.Cards.ChangeName(new CardId(message.CardId), message.Name);
        }

        public async void Handle(CardDescriptionChanged message)
        {
            using (new ProgressService(_progress))
                await _api.Async.Cards.ChangeDescription(new CardId(message.CardId), message.Description);
        }

        public async void Handle(CheckItemChanged message)
        {
            using (new ProgressService(_progress))
                await _api.Async.Cards.ChangeCheckItemState(new CardId(message.CardId),
                                                            new ChecklistId(message.ChecklistId),
                                                            new CheckItemId(message.CheckItemId),
                                                            message.Value);
        }

        public async void Handle(CardDueDateChanged message)
        {
            using (new ProgressService(_progress))
            {
                await _api.Async.Cards.ChangeDueDate(new CardId(message.CardId), message.DueDate);
            }
        }

        public async void Handle(CardLabelsChanged message)
        {
            using (new ProgressService(_progress))
            {
                foreach (var added in message.LabelsAdded)
                    await _api.Async.Cards.AddLabel(new CardId(message.CardId), added.Color);
                foreach (var removed in message.LabelsRemoved)
                    await _api.Async.Cards.RemoveLabel(new CardId(message.CardId), removed.Color);
            }
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
    }
}