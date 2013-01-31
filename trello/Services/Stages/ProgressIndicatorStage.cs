using System.Threading.Tasks;
using JetBrains.Annotations;
using trellow.api.Data;
using trellow.api.Data.Stages;

namespace trello.Services.Stages
{
    [UsedImplicitly]
    public class ProgressIndicatorStage : RequestPipelineStage
    {
        private readonly IProgressService _progressService;

        public ProgressIndicatorStage(IProgressService progressService)
        {
            _progressService = progressService;
        }

        public override async Task<ResponseContext<T>> Handle<T>(ResponseContext<T> context)
        {
            _progressService.Show();

            context = await ContinueIfPossible(context);

            _progressService.Hide();

            return context;
        }
    }
}