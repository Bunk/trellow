using System.Collections.Generic;
using System.Threading.Tasks;
using trello.Services.Models;

namespace trello.Services.Data
{
    public interface ICardService
    {
        Task<IEnumerable<Card>> Mine();
        Task<IEnumerable<Card>> InList(string listId);
        Task<Card> WithId(string id);
    }
}