using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using trellow.api.Models;

namespace trello.ViewModels
{
    public class ChangeCardLabelsViewModel : DialogViewModel
    {
        public Action<List<trellow.api.Models.Label>> Accepted { get; set; }

        public IList<Label> Labels { get; set; }

        public ChangeCardLabelsViewModel(object root, LabelNames labelNames, IEnumerable<LabelViewModel> labels) : base(root)
        {
            var lab = labels.ToArray();
            Labels = new List<Label>
            {
                new Label { Color = "green", Name = labelNames.Green, Selected = lab.Any(l => l.Color == "green")},
                new Label { Color = "yellow", Name = labelNames.Yellow, Selected = lab.Any(l => l.Color == "yellow") },
                new Label { Color = "orange", Name = labelNames.Orange, Selected = lab.Any(l => l.Color == "orange") },
                new Label { Color = "red", Name = labelNames.Red, Selected = lab.Any(l => l.Color == "red") },
                new Label { Color = "purple", Name = labelNames.Purple, Selected = lab.Any(l => l.Color == "purple") },
                new Label { Color = "blue", Name = labelNames.Blue, Selected = lab.Any(l => l.Color == "blue") }
            };
        }

        public void Toggle(Label label)
        {
            label.Selected = !label.Selected;
        }

        public void Accept()
        {
            var labels = Labels
                .Where(l => l.Selected)
                .Select(l => new trellow.api.Models.Label {Color = l.Color})
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
