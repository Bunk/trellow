namespace trellow.api.Data
{
    public interface IRequestPipeline
    {
        IRequestPipelineStage Build();
    }
}