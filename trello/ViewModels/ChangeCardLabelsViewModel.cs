using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using JetBrains.Annotations;
using trello.Services;
using trello.Services.Handlers;
using trellow.api;
using trellow.api.Cards;

namespace trello.ViewModels
{
    public class ChangeCardLabelsViewModel : DialogViewModel
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly ITrello _api;
        private readonly IProgressService _progress;
        private IList<Color> _selected;

        public string CardId { private get; set; }

        public IObservableCollection<Label> Labels { get; set; }

        public ChangeCardLabelsViewModel(object root, string cardId, IEventAggregator eventAggregator, ITrello api, IProgressService progress) : base(root)
        {
            _eventAggregator = eventAggregator;
            _api = api;
            _progress = progress;

            CardId = cardId;

            Labels = new BindableCollection<Label>();
        }

        protected override async void OnInitialize()
        {
            _progress.Show("Loading names...");
            try
            {
                var board = await _api.Boards.ForCard(new CardId(CardId));
                foreach (var lbl in Labels)
                {
                    string name;
                    if (board.LabelNames.TryGetValue(lbl.Color, out name))
                        lbl.Name = name;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("The label names were unable to be loaded.  Please " +
                                "ensure that you have an active internet connection.");
            }
            finally
            {
                _progress.Hide();
            }
        }

        public ChangeCardLabelsViewModel Initialize(IEnumerable<Color> selected)
        {
            _selected = selected.ToList();

            Labels.Add(CreateLabel(Color.Green));
            Labels.Add(CreateLabel(Color.Yellow));
            Labels.Add(CreateLabel(Color.Orange));
            Labels.Add(CreateLabel(Color.Red));
            Labels.Add(CreateLabel(Color.Purple));
            Labels.Add(CreateLabel(Color.Blue));
            
            return this;
        }

        private Label CreateLabel(Color color)
        {
            var lbl = new Label(_eventAggregator);
            using (new SuppressNotificationScope(lbl))
            {
                lbl.CardId = CardId;
                lbl.Color = color;
                lbl.Selected = _selected.Any(c => c == color);
            }
            return lbl;
        }

        [UsedImplicitly]
        public void Toggle(Label label)
        {
            label.Toggle();
        }

        public class Label : PropertyChangedBase
        {
            private bool _selected;
            private string _name;

            public string CardId { get; set; }

            public Color Color { get; set; }

            public string Name
            {
                get { return _name; }
                set
                {
                    if (value == _name) return;
                    _name = value;
                    NotifyOfPropertyChange(() => Name);
                }
            }

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

            public Label(IEventAggregator eventAggregator)
            {
                PropertyChanged += (sender, args) =>
                {
                    if (!IsNotifying)
                        return;

                    if (args.PropertyName != "Selected") 
                        return;

                    if (Selected)
                        eventAggregator.Publish(new CardLabelAdded {CardId = CardId, Color = Color, Name = Name});
                    else
                        eventAggregator.Publish(new CardLabelRemoved {CardId = CardId, Color = Color, Name = Name});
                };
            }

            public bool Toggle()
            {
                return (Selected = !Selected);
            }
        }
    }
}
