using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using TrelloNet;

namespace trello.ViewModels
{
    public class ChangeCardLabelsViewModel : DialogViewModel
    {
        private readonly Dictionary<Color, string> _labelNames;
        private readonly IList<LabelViewModel> _labels;

        public Action<List<Card.Label>> Accepted { get; set; }

        public IList<Label> Labels { get; set; }

        public ChangeCardLabelsViewModel(object root, Dictionary<Color, string> labelNames, IEnumerable<LabelViewModel> labels) : base(root)
        {
            _labelNames = labelNames;
            _labels = labels.ToList();

            Labels = new List<Label>
            {
                CreateLabel(Color.Green),
                CreateLabel(Color.Yellow),
                CreateLabel(Color.Orange),
                CreateLabel(Color.Red),
                CreateLabel(Color.Purple),
                CreateLabel(Color.Blue)
            };
        }

        private Label CreateLabel(Color color)
        {
            return new Label
            {
                Color = color.ToString(),
                Name = _labelNames[color],
                Selected = _labels.Any(l => l.Color == color.ToString())
            };
        }

        public void Toggle(Label label)
        {
            label.Selected = !label.Selected;
        }

        public void Confirm()
        {
            var labels = Labels
                .Where(l => l.Selected)
                .Select(l => new Card.Label
                {
                    Color = (Color)Enum.Parse(typeof(Color), l.Color), 
                    Name = l.Name
                })
                .ToList();

            Accepted(labels);
            TryClose();
        }

        public class Label : PropertyChangedBase
        {
            private bool _selected;

            public string Name { get; set; }

            public string Color { get; set; }

            public bool Selected
            {
                get { return _selected; }
                set
                {
                    if (value.Equals(_selected)) return;
                    _selected = value;
                    NotifyOfPropertyChange(() => Selected);
                }
            }
        }
    }
}
