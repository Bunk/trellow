using System.Threading.Tasks;
using trellow.api.Models;
using Strilanc.Value;

namespace trellow.api.Data.Services
{
    public interface IProfileService
    {
        Task<May<Profile>> Mine();
    }
}