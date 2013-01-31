using System;
using JetBrains.Annotations;
using trello.Services.Stages;
using trellow.api.Data;
using trellow.api.Data.Stages;

namespace trello.Services.Pipelines
{
    [UsedImplicitly]
    public class ConnectedRequestPipeline : IRequestPipeline
    {
        private readonly Func<CallExternalApiStage> _callApiStage;
        private readonly Func<CacheStage> _cacheStage;
        private readonly Func<ProgressIndicatorStage> _progressStage;

        public ConnectedRequestPipeline(Func<CallExternalApiStage> callApiStage,
                                        Func<CacheStage> cacheStage,
                                        Func<ProgressIndicatorStage> progressStage)
        {
            _callApiStage = callApiStage;
            _cacheStage = cacheStage;
            _progressStage = progressStage;
        }

        public IRequestPipelineStage Build()
        {
            var _1 = _progressStage();
            var _2 = _cacheStage();
            var _3 = _callApiStage();

            _1.Then(_2.Then(_3));

            return _1;
        }
    }
}