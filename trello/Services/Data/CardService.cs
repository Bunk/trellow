using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using trello.Services.Models;

namespace trello.Services.Data
{
    [UsedImplicitly]
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
                    .AddUrlSegment("id", listId)
                    .AddParameter("attachments", "true")
                    .AddParameter("attachments_fields", "previews"));
        }

        public async Task<Card> WithId(string id)
        {
            return await Processor.Execute<Card>(
                Request("cards/{id}")
                    .AddUrlSegment("id", id));
        }
    }

    [UsedImplicitly]
    public class JsonCardService : JsonServiceBase, ICardService
    {
        public async Task<IEnumerable<Card>> Mine()
        {
            const string file = "SampleData/cards/cards-mine.json";
            return await ReadFile<IEnumerable<Card>>(file);
        }

        public async Task<IEnumerable<Card>> InList(string listId)
        {
            var file = string.Format("SampleData/lists/list-{0}.json", listId);
            return await ReadFile<IEnumerable<Card>>(file);
        }

        public async Task<Card> WithId(string id)
        {
            var file = string.Format("SampleData/cards/card-{0}.json", id);
            return await ReadFile<Card>(file);
        }
    }
}