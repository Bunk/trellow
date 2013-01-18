using System.Threading.Tasks;
using trellow.api.Models;

namespace trellow.api.Data
{
    public class ProfileService : ServiceBase, IProfileService
    {
        public ProfileService(IRequestProcessor processor) : base(processor)
        {
        }


        public async Task<Profile> Mine()
        {
            return await Processor.Execute<Profile>(
                Request("members/me"));
        }
    }

    public class JsonProfileService : JsonServiceBase, IProfileService
    {
        public Task<Profile> Mine()
        {
            return ReadFile<Profile>("SampleData/members/members-me.json");
        }
    }
}
