using System.Threading.Tasks;
using trellow.api.Data.Services;

namespace trellow.api.Data.Stages
{
    public class CallExternalApiStage : RequestPipelineStage
    {
        public override async Task<ResponseContext<T>> Handle<T>(ResponseContext<T> context)
        {
            // If the data has already been set, we don't call the service
            if (!context.Data.HasValue)
            {
                var response = await context.Client.ExecuteAwaitable<T>(context.Request);
                context.Response = response;
                context.Data = response.Data;
            }

            return await ContinueIfPossible(context);
        }
    }
}