using System.Threading.Tasks;
using trello.Services.Models;
using trello.ViewModels;

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

    public class MockProfileService : MockServiceBase, IProfileService
    {
        public MockProfileService(IProgressService progressService) : base(progressService)
        {
        }

        public Task<Profile> Mine()
        {
            return Execute(() => Task.Run(() => new Profile
            {
                Bio = "This is some person's mock profile information.  It could be quite long.",
                FullName = "Mock User",
                Username = "mockuser",
                Email = "mocked@mock.com"
            }));
        }
    }
}
