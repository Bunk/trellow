using System.Collections.Generic;
using System.Threading.Tasks;
using trello.Services.Models;

namespace trello.Services.Data
{
    public interface IBoardService
    {
        Task<List<Board>> Mine();

        Task<Board> WithId(string id);
    }
}