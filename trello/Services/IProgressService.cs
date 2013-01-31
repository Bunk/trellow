namespace trello.Services
{
    public interface IProgressService
    {
        void Show();
        void Show(string text);
        void Hide();
    }
}