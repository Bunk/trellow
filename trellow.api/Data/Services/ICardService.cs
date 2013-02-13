using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Strilanc.Value;
using trellow.api.Models;

namespace trellow.api.Data.Services
{
    public interface ICardService
    {
        Task<May<List<Card>>> Mine();
        Task<May<List<Card>>> InList(string listId);
        Task<May<Card>> WithId(string id);

        Task UpdateName(string id, string name);
        Task UpdateDescription(string id, string description);
        Task UpdateCheckedItem(string id, string checklistId, string itemId, bool value);
        Task UpdateDueDate(string id, DateTime? date);
        Task UpdateLabels(string id, IEnumerable<Label> labelsAdded, IEnumerable<Label> labelsRemoved);
    }
}