namespace Trellow.Services.UI
{
    public interface IProgressService
    {
        void Show();
        void Show(string text);
        void Hide();
    }
}