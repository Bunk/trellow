using System;

namespace Trellow.Services.UI
{
    public class ProgressScope : IDisposable
    {
        private readonly IProgressService _progress;

        public ProgressScope(IProgressService progress, string message = "Loading...")
        {
            _progress = progress;
            _progress.Show(message);
        }

        public void Dispose()
        {
            _progress.Hide();
        }
    }
}