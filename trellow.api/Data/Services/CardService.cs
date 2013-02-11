using System;
using System.Collections.Generic;
using System.Globalization;
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
                    .AddParameter("board_fields", "name,labelNames")
                    .AddParameter("list", "true"));
        }

        public Task UpdateName(string id, string name)
        {
            return Processor.Execute<Card>(
                Update("cards/{id}")
                    .AddUrlSegment("id", id)
                    .AddParameter("name", name));
        }

        public Task UpdateDescription(string id, string description)
        {
            return Processor.Execute<Card>(
                Update("cards/{id}")
                    .AddUrlSegment("id", id)
                    .AddParameter("desc", description));
        }

        public Task UpdateCheckedItem(string id, string checklistId, string itemId, bool value)
        {
            return Processor.Execute<object>(
                Update("cards/{id}/checklist/{checklist}/checkItem/{item}")
                    .AddUrlSegment("id", id)
                    .AddUrlSegment("checklist", checklistId)
                    .AddUrlSegment("item", itemId)
                    .AddParameter("state", value ? "complete" : "incomplete"));
        }

        public Task UpdateDueDate(string id, DateTime? date)
        {
            return Processor.Execute<Card>(
                Update("cards/{id}")
                    .AddUrlSegment("id", id)
                    .AddParameter("due", FormatDate(date)));
        }

        public Task UpdateLabels(string id, IEnumerable<Label> labels)
        {
            var formatted = string.Join(",", labels.Select(l => l.Name));

            return Processor.Execute<Card>(
                Update("cards/{id}")
                    .AddUrlSegment("id", id)
                    .AddParameter("labels", "[" + formatted + "]"));
        }

        private static string FormatDate(DateTime? date)
        {
            return date.HasValue
                       ? date.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture)
                       : "null";
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

        public Task UpdateName(string id, string name)
        {
            throw new System.NotImplementedException();
        }

        public Task UpdateDescription(string id, string description)
        {
            throw new System.NotImplementedException();
        }

        public Task UpdateCheckedItem(string id, string checklistId, string itemId, bool value)
        {
            throw new System.NotImplementedException();
        }

        public Task UpdateDueDate(string id, DateTime? date)
        {
            throw new NotImplementedException();
        }

        public Task UpdateLabels(string id, IEnumerable<Label> labels)
        {
            throw new NotImplementedException();
        }
    }
}