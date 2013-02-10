using Caliburn.Micro;
using trellow.api.Data.Services;

namespace trello.Services.Handlers
{
    public class CardDetailCommandHandler : IHandle<NameChanged>, IHandle<DescriptionChanged>
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
    }
}