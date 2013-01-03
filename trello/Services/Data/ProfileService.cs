using System.Threading.Tasks;
using trello.Services.Models;

namespace trello.Services.Data
{
    public interface IProfileService
    {
        Task<Profile> Mine();
    }

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
}
