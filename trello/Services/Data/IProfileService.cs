using System.Threading.Tasks;
using trello.Services.Models;

namespace trello.Services.Data
{
    public interface IProfileService
    {
        Task<Profile> Mine();
    }
}