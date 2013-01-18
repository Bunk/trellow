using System.Collections.Generic;
using System.Threading.Tasks;
using trellow.api.Models;

namespace trellow.api.Data
{
    public interface ICardService
    {
        Task<List<Card>> Mine();
        Task<List<Card>> InList(string listId);
        Task<Card> WithId(string id);
    }
}