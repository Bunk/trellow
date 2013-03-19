using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace trello.Interactions
{
    public class InteractionManager
    {
        private readonly IList<IInteraction> _interactions;

        public InteractionManager()
        {
            _interactions = new List<IInteraction>();
        }

        public void AddInteraction(IInteraction interaction)
        {
            _interactions.Add(interaction);
            interaction.Activated += (sender, args) => Activated(sender);
            interaction.Deactivated += (sender, args) => Deactivated();
        }

        public void AddElement(FrameworkElement element)
        {
            foreach (var interaction in _interactions)
                interaction.AddElement(element);
        }

        private void Activated(object sender)
        {
            foreach (var interaction in _interactions.Where(i => i != sender))
                interaction.IsEnabled = false;
        }

        private void Deactivated()
        {
            foreach (var interaction in _interactions)
                interaction.IsEnabled = true;
        }
    }
}