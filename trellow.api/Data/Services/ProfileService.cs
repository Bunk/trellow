using System.Threading.Tasks;
using trellow.api.Models;
using Strilanc.Value;

namespace trellow.api.Data.Services
{
    public class ProfileService : ServiceBase, IProfileService
    {
        public ProfileService(IRequestProcessor processor) : base(processor)
        {
        }
        
        public async Task<May<Profile>> Mine()
        {
            return await Processor.Execute<Profile>(Request("members/me"));
        }
    }

    public class JsonProfileService : JsonServiceBase, IProfileService
    {
        public async Task<May<Profile>> Mine()
        {
            var value = await ReadFile<Profile>("SampleData/members/members-me.json");
            return value;
        }
    }
}
