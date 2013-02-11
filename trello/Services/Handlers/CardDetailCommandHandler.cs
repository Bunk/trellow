using Caliburn.Micro;
using trellow.api.Data.Services;

namespace trello.Services.Handlers
{
    public class CardDetailCommandHandler : IHandle<NameChanged>, IHandle<DescriptionChanged>, IHandle<CheckItemChanged>
    {
        private readonly ICardService _cardService;

        public CardDetailCommandHandler(IEventAggregator eventAggregator, ICardService cardService)
        {
            _cardService = cardService;

            eventAggregator.Subscribe(this);
        }

        public async void Handle(NameChanged message)
        {
            await _cardService.UpdateName(message.CardId, message.Name);
        }

        public async void Handle(DescriptionChanged message)
        {
            await _cardService.UpdateDescription(message.CardId, message.Description);
        }

        public async void Handle(CheckItemChanged message)
        {
            await _cardService.UpdateCheckedItem(message.CardId, message.ChecklistId, message.CheckItemId, message.Value);
        }
    }
}