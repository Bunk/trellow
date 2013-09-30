using System.Collections.Generic;
using Caliburn.Micro;
using JetBrains.Annotations;
using Trellow.Diagnostics;
using Trellow.Events;
using Trellow.Services.UI;
using trellow.api;
using trellow.api.Boards;
using trellow.api.Cards;
using trellow.api.Lists;

namespace Trellow.Handlers.Cards
{
    [UsedImplicitly]
    public class MovementHandler : AbstractHandler,
                                   IHandle<CardArchived>,
                                   IHandle<CardDeleted>,
                                   IHandle<CardPriorityChanged>,
                                   IHandle<CardMovedToList>,
                                   IHandle<CardMovedToBoard>
    {
        public MovementHandler(IEventAggregator events, ITrello api, IProgressService progress)
            : base(events, api, progress)
        {
        }

        public void Handle(CardArchived message)
        {
            Analytics.TagEvent("Archive_Card");
            Handle(api => api.Cards.Archive(new CardId(message.CardId)));
        }

        public void Handle(CardDeleted message)
        {
            Analytics.TagEvent("Deleted_Card");
            Handle(api => api.Cards.Delete(new CardId(message.CardId)));
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

        public void Handle(CardMovedToList message)
        {
            Analytics.TagEvent("Move_Card", new Dictionary<string, string>
            {
                { "Method", "Drag_and_Drop" }
            });
            Handle(api => api.Cards.Move(new CardId(message.Card.Id), new ListId(message.DestinationListId)));
        }

        public void Handle(CardMovedToBoard message)
        {
            Analytics.TagEvent("Move_Card", new Dictionary<string, string>
            {
                { "Method", "Action_Link" }
            });
            Handle(api => api.Cards.Move(new CardId(message.CardId),
                                         new BoardId(message.BoardId),
                                         new ListId(message.ListId)));
        }
    }
}