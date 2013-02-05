using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Strilanc.Value;
using trellow.api.Models;

namespace trellow.api.Data.Services
{
    [UsedImplicitly]
    public class CardService : ServiceBase, ICardService
    {
        public CardService(IRequestProcessor processor) : base(processor)
        {
        }

        public async Task<May<List<Card>>> Mine()
        {
            var profile = await Processor.Execute<Profile>(
                Request("members/me")
                    .AddParameter("boards", "open")
                    .AddParameter("board_fields", "name,closed,idOrganization,pinned,prefs")
                    .AddParameter("board_lists", "open")
                    .AddParameter("cards", "visible")
                    .AddParameter("card_fields", "badges,closed,due,idAttachmentCover,idList,idBoard,idMembers,idShort,labels,name")
                    .AddParameter("card_members", "true")
                    .AddParameter("card_attachments", "true")
                    .AddParameter("organizations", "all")
                    .AddParameter("organization_fields", "displayName,name"));

            return profile.Select(p =>
            {
                var list = new List<Card>();
                foreach (var card in p.Cards)
                {
                    card.Board = p.Boards.FirstOrDefault(b => b.Id == card.IdBoard);
                    if (card.Board != null)
                        card.List = card.Board.Lists.FirstOrDefault(l => l.Id == card.IdList);

                    list.Add(card);
                }
                return list;
            });
        }

        public Task<May<List<Card>>> InList(string listId)
        {
            return Processor.Execute<List<Card>>(
                Request("lists/{id}/cards")
                    .AddUrlSegment("id", listId)
                    .AddParameter("members", "true")
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
                                             "createCard,deleteAttachmentFromCard,removeChecklistFromCard,removeMemberFromCard")
                    .AddParameter("actions_limit", "50")
                    .AddParameter("checklists", "all")
                    .AddParameter("checkItemStates", "true")
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