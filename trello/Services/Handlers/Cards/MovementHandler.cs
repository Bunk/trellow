﻿using BugSense;
using Caliburn.Micro;
using JetBrains.Annotations;
using trellow.api;
using trellow.api.Cards;
using trellow.api.Lists;

namespace trello.Services.Handlers.Cards
{
    [UsedImplicitly]
    public class MovementHandler : AbstractHandler,
                                   IHandle<CardArchived>,
                                   IHandle<CardDeleted>,
                                   IHandle<CardPriorityChanged>,
                                   IHandle<CardMovingFromList>
    {
        public MovementHandler(IEventAggregator events, ITrello api, IProgressService progress)
            : base(events, api, progress)
        {
        }

        public void Handle(CardArchived message)
        {
            BugSenseHandler.Instance.SendEvent("Archive card");
            Handle(api => api.Cards.Archive(new CardId(message.CardId)));
        }

        public void Handle(CardDeleted message)
        {
            BugSenseHandler.Instance.SendEvent("Delete card");
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

        public void Handle(CardMovingFromList message)
        {
            BugSenseHandler.Instance.SendEvent("Move card to list");
            Handle(api => api.Cards.Move(new CardId(message.Card.Id), new ListId(message.DestinationListId)));
        }
    }
}