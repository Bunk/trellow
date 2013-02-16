using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Strilanc.Value;
using trellow.api.Data.Stages;

namespace trellow.api.Data.Services
{
    public class TrelloCoordinator
    {
        private readonly IRequestPipeline _pipelineFactory;

        public TrelloCoordinator(IRequestPipeline pipelineFactory)
        {
            _pipelineFactory = pipelineFactory;
        }

        public async Task<May<T>> Execute<T>(Func<Task<T>> func)
        {
            var context = new RequestContext<T>
            {
                Execute = func
            };

            var pipeline = _pipelineFactory.Build();
            context = await pipeline.Handle(context);
            return context.Data;
        }
    }
}
