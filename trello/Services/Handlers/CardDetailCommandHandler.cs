using Caliburn.Micro;
using trellow.api.Data.Services;

namespace trello.Services.Handlers
{
    public class CardDetailCommandHandler : IHandle<CardNameChanged>,
                                            IHandle<CardDescriptionChanged>,
                                            IHandle<CheckItemChanged>,
                                            IHandle<CardDueDateChanged>
    {
        private readonly ICardService _cardService;

        public CardDetailCommandHandler(IEventAggregator eventAggregator, ICardService cardService)
        {
            _cardService = cardService;

            eventAggregator.Subscribe(this);
        }

        public async void Handle(CardNameChanged message)
        {
            await _cardService.UpdateName(message.CardId, message.Name);
        }

        public async void Handle(CardDescriptionChanged message)
        {
            await _cardService.UpdateDescription(message.CardId, message.Description);
        }

        public async void Handle(CheckItemChanged message)
        {
            await _cardService.UpdateCheckedItem(message.CardId, message.ChecklistId,
                                                 message.CheckItemId, message.Value);
        }

        public async void Handle(CardDueDateChanged message)
        {
            await _cardService.UpdateDueDate(message.CardId, message.DueDate);
        }
    }
}