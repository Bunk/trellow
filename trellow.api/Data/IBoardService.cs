using System.Collections.Generic;
using System.Threading.Tasks;
using trellow.api.Models;
using Strilanc.Value;

namespace trellow.api.Data
{
    public interface IBoardService
    {
        Task<May<List<Board>>> Mine();

        Task<May<Board>> WithId(string id);
    }
}