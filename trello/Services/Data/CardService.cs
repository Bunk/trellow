using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using trello.Services.Models;
using trello.ViewModels;

namespace trello.Services.Data
{
    public class CardService : ServiceBase, ICardService
    {
        public CardService(IRequestProcessor processor) : base(processor)
        {
        }

        public async Task<IEnumerable<Card>> Mine()
        {
            return await Processor.Execute<List<Card>>(
                Request("members/my/cards/open")
                    .AddParameter("members", "false"));
        }

        public async Task<IEnumerable<Card>> InList(string listId)
        {
            return await Processor.Execute<List<Card>>(
                Request("lists/{id}/cards")
                    .AddUrlSegment("id", listId));
        }
    }

    public class MockCardService : MockServiceBase, ICardService
    {
        public MockCardService(IProgressService progressService) : base(progressService)
        {
        }

        public Task<IEnumerable<Card>> Mine()
        {
            return Execute(() => Task.Run(() =>
            {
                var cards = new List<Card>
                {
                    new Card
                    {
                        Id = "CARD-1",
                        IdBoard = "BOARD-1",
                        IdList = "TODO",
                        Desc = "Description for Card 1",
                        Badges = new Badges
                        {
                            Attachments = 1,
                            CheckItems = 2,
                            CheckItemsChecked = 1,
                            Comments = 3,
                            Description = true,
                            Due = DateTime.UtcNow,
                            Subscribed = true,
                            ViewingMemberVoted = true,
                            Votes = 4
                        }
                    }
                };
                return (IEnumerable<Card>) cards;
            }));
        }

        public Task<IEnumerable<Card>> InList(string listId)
        {
            IEnumerable<Card> cards = new List<Card>
            {
                new Card
                {
                    Id = "CARD-1",
                    IdList = listId,
                    Desc = "Finalize Trellow work",
                    Name = "Finalize Trellow work",
                    Due = new DateTime(2013, 2, 1),
                    Badges = new Badges
                    {
                        CheckItems = 5,
                        CheckItemsChecked = 3,
                        Due = new DateTime(2013, 2, 1)
                    }
                }
            };

            return Run(() => cards);
        } 
    }
}