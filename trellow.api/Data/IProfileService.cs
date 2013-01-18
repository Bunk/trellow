using System.Threading.Tasks;
using trellow.api.Models;

namespace trellow.api.Data
{
    public interface IProfileService
    {
        Task<Profile> Mine();
    }
}