using System.Collections.Generic;
using System.Threading.Tasks;
using trello.Services.Models;

namespace trello.Services.Data
{
    public interface IBoardService
    {
        Task<IEnumerable<Board>> Mine();
    }
}