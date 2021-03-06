using System.Threading.Tasks;
using trellow.api.Data.Stages;

namespace trellow.api.Data
{
    public interface IRequestPipelineStage
    {
        Task<ResponseContext<T>> Handle<T>(ResponseContext<T> context);

        Task<RequestContext<T>> Handle<T>(RequestContext<T> context);

        IRequestPipelineStage Then(IRequestPipelineStage next);
    }

    public abstract class RequestPipelineStage : IRequestPipelineStage
    {
        private IRequestPipelineStage _nextHandler;

        public abstract Task<ResponseContext<T>> Handle<T>(ResponseContext<T> context);

        public abstract Task<RequestContext<T>> Handle<T>(RequestContext<T> context);

        public IRequestPipelineStage Then(IRequestPipelineStage nextHandler)
        {
            _nextHandler = nextHandler;
            return this;
        }

        protected async Task<ResponseContext<T>> ContinueIfPossible<T>(ResponseContext<T> context)
        {
            if (_nextHandler != null)
                return await _nextHandler.Handle(context);
            return context;
        }

        protected async Task<RequestContext<T>> ContinueIfPossible<T>(RequestContext<T> context)
        {
            if (_nextHandler != null)
                return await _nextHandler.Handle(context);

            return context;
        }
    }
}