using trellow.api.Models;

namespace trello.ViewModels
{
    public class LabelViewModel
    {
        public string Color { get; set; }

        public string Name { get; set; }

        public LabelViewModel(Label lbl)
        {
            Color = lbl.Color;
            Name = lbl.Name;
        }

        public LabelViewModel(string color, string name)
        {
            Color = color;
            Name = name;
        }
    }
}