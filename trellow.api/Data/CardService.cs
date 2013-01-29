using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Strilanc.Value;
using trellow.api.Models;

namespace trellow.api.Data
{
    [UsedImplicitly]
    public class CardService : ServiceBase, ICardService
    {
        public CardService(IRequestProcessor processor) : base(processor)
        {
        }

        public Task<May<List<Card>>> Mine()
        {
            return Processor.Execute<List<Card>>(
                Request("members/my/cards/open")
                    .AddParameter("members", "false"));
        }

        public Task<May<List<Card>>> InList(string listId)
        {
            return Processor.Execute<List<Card>>(
                Request("lists/{id}/cards")
                    .AddUrlSegment("id", listId)
                    .AddParameter("attachments", "true")
                    .AddParameter("attachments_fields", "previews"));
        }

        public Task<May<Card>> WithId(string id)
        {
            return Processor.Execute<Card>(
                Request("cards/{id}")
                    .AddUrlSegment("id", id)
                    .AddParameter("attachments", "true")
                    .AddParameter("members", "true")
                    .AddParameter("member_fields", "fullName,initials,memberType,username,avatarHash,bio,status")
                    .AddParameter("actions", "addAttachmentToCard,addChecklistToCard,addMemberToCard,commentCard," +
                                             "copyCommentCard,createCard,copyCard")
                    .AddParameter("actions_limit", "50")
                    .AddParameter("checklists", "all")
                    .AddParameter("board", "true")
                    .AddParameter("list", "true"));
        }
    }

    [UsedImplicitly]
    public class JsonCardService : JsonServiceBase, ICardService
    {
        public async Task<May<List<Card>>> Mine()
        {
            const string file = "SampleData/cards/cards-mine.json";
            return await ReadFile<List<Card>>(file);
        }

        public async Task<May<List<Card>>> InList(string listId)
        {
            var file = string.Format("SampleData/lists/list-{0}.json", listId);
            return await ReadFile<List<Card>>(file);
        }

        public async Task<May<Card>> WithId(string id)
        {
            var file = string.Format("SampleData/cards/card-{0}.json", id);
            return await ReadFile<Card>(file);
        }
    }
}