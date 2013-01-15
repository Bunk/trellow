using System.Collections.Generic;
using System.Threading.Tasks;
using trello.Services.Models;

namespace trello.Services.Data
{
    public interface ICardService
    {
        Task<List<Card>> Mine();
        Task<List<Card>> InList(string listId);
        Task<Card> WithId(string id);
    }
}