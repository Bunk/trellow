using System;
using Caliburn.Micro;

namespace trello.ViewModels
{
    public abstract class ResultBase : IResult
    {
        public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };  

        public abstract void Execute(ActionExecutionContext context);

        protected virtual void OnCompleted()
        {
            OnCompleted(new ResultCompletionEventArgs());
        }

        protected virtual void OnCompleted(ResultCompletionEventArgs e)
        {
            Caliburn.Micro.Execute.OnUIThread(() => Completed(this, e));
        }

        protected virtual void OnError(Exception ex)
        {
            OnCompleted(new ResultCompletionEventArgs
            {
                Error = ex
            });
        }

        protected virtual void OnCancelled()
        {
            OnCompleted(new ResultCompletionEventArgs
            {
                WasCancelled = true
            });
        }
    }
}