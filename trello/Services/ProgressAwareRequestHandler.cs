using RestSharp;
using trello.ViewModels;
using trellow.api.Data;

namespace trello.Services
{
    public class ProgressAwareRequestHandler : IHandleRequests
    {
        private readonly IProgressService _progressService;

        public ProgressAwareRequestHandler(IProgressService progressService)
        {
            _progressService = progressService;
        }

        public void BeforeRequest<T>(IRestRequest request)
        {
            _progressService.Show();
        }

        public void AfterRequest<T>(IRestRequest request, T result)
        {
            _progressService.Hide();
        }
    }
}