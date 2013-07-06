using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace trello.Interactions
{
    /// <summary>
    /// Responsible for managing the collaborations between UX interactions.
    /// </summary>
    public class InteractionManager : InteractionBase
    {
        private readonly IList<IInteraction> _interactions;

        public InteractionManager()
        {
            _interactions = new List<IInteraction>();
        }

        /// <summary>
        /// Adds an interaction to the manager.  Interactions listen to UI
        /// events and operate on them.
        /// </summary>
        public virtual void AddInteraction(IInteraction interaction)
        {
            _interactions.Add(interaction);

            interaction.IsEnabled = true;
            interaction.Activated += (sender, args) => ChildActivated(sender);
            interaction.Deactivated += (sender, args) => ChildDeactivated();
            //todo: Possible memory leaks--check on weak references for these
        }

        /// <summary>
        /// Adds an element to the list of elements that collaborate in the
        /// interaction.
        /// </summary>
        public override void AddElement(FrameworkElement element)
        {
            foreach (var interaction in _interactions)
                interaction.AddElement(element);
        }

        protected bool AnyChildrenActive
        {
            get { return _interactions.Any(i => i.IsActive); }
        }

        protected void EachChild(Action<IInteraction> action, Func<IInteraction, bool> where = null)
        {
            var enumerable = _interactions;
            if (where != null)
                enumerable = enumerable.Where(where).ToList();

            foreach (var interaction in enumerable)
                action(interaction);
        }

        protected virtual void ChildActivated(object sender)
        {
            // disable all interactions except the one that sent an activation signal
            EachChild(i => i.IsEnabled = false, i => i != sender);
        }

        protected virtual void ChildDeactivated()
        {
            // re-enable all interactions so that they can now handle events
            EachChild(i => i.IsEnabled = true);
        }
    }
}