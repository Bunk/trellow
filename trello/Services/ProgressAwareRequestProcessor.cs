using RestSharp;
using trello.ViewModels;
using trellow.api.Data;
using trellow.api.OAuth;

namespace trello.Services
{
    public class ProgressAwareRequestProcessor : RequestProcessor
    {
        private readonly IProgressService _progressService;

        public ProgressAwareRequestProcessor(IOAuthClient oauthClient, IProgressService progressService)
            : base(oauthClient)
        {
            _progressService = progressService;
        }

        protected override void OnQueryStart<T>(IRestRequest request)
        {
            _progressService.Show();
        }

        protected override void OnQueryComplete<T>(IRestRequest request, T result)
        {
            _progressService.Hide();
        }
    }
}