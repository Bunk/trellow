namespace trello.ViewModels
{
    public class LabelViewModel
    {
        public string Color { get; set; }

        public string Name { get; set; }

        public LabelViewModel(string color, string name)
        {
            Color = color;
            Name = name;
        }
    }
}