using System;
using System.Threading;
using System.Threading.Tasks;
using trello.ViewModels;

namespace trello.Services.Data
{
    public abstract class MockServiceBase
    {
        private readonly IProgressService _progressService;

        public MockServiceBase(IProgressService progressService)
        {
            _progressService = progressService;
        }

        public async Task<T> Execute<T>(Func<Task<T>> action)
        {
            _progressService.Show();

            var results = await action();

            _progressService.Hide();

            return results;
        }

        protected Task<T> Run<T>(Func<T> func)
        {
            return Execute(() => Task.Run(func));
        }

        protected Task<T> Run<T>(Func<T> func, TimeSpan delay)
        {
            return Execute(() => Task.Run(() =>
            {
                var val = func();
                Thread.Sleep(delay);
                return val;
            }));
        }
    }
}