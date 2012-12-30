namespace trello.ViewModels
{
    public interface IProgressService
    {
        void Show();
        void Show(string text);
        void Hide();
    }
}