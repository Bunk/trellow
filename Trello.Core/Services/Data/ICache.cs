using System.Threading.Tasks;
using Strilanc.Value;

namespace Trellow.Services.Data
{
    public interface ICache
    {
        bool Contains(string key);

        bool Expired(string key);

        May<T> Get<T>(string key);

        May<T> Set<T>(string key, May<T> value);

        Task<bool> Initialize();
    }
}