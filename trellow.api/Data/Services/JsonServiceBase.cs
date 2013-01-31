using System.IO;
using System.Threading.Tasks;

namespace trellow.api.Data.Services
{
    public abstract class JsonServiceBase
    {
        protected static async Task<T> ReadFile<T>(string filename)
        {
            using (var stream = File.OpenText(filename))
            {
                var content = await stream.ReadToEndAsync();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(content);
            }
        }
    }
}