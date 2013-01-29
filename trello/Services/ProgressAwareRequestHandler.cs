using JetBrains.Annotations;
using trello.ViewModels;
using trellow.api.Data;

namespace trello.Services
{
    [UsedImplicitly]
    public class ProgressAwareRequestHandler : IHandleRequests
    {
        private readonly IProgressService _progressService;

        public ProgressAwareRequestHandler(IProgressService progressService)
        {
            _progressService = progressService;
        }

        public RequestContext<T> BeforeRequest<T>(RequestContext<T> context)
        {
            _progressService.Show();
            return context;
        }

        public ResponseContext<T> AfterRequest<T>(ResponseContext<T> context)
        {
            _progressService.Hide();
            return context;
        }
    }
}