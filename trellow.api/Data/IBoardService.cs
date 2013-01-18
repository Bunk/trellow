using System.Collections.Generic;
using System.Threading.Tasks;
using trellow.api.Models;

namespace trellow.api.Data
{
    public interface IBoardService
    {
        Task<List<Board>> Mine();

        Task<Board> WithId(string id);
    }
}