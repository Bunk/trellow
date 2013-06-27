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
        public void AddInteraction(IInteraction interaction)
        {
            _interactions.Add(interaction);
            interaction.Activated += (sender, args) => ChildActivated(sender);
            interaction.Deactivated += (sender, args) => ChildDeactivated();
            interaction.Completed += (sender, args) => ChildCompleted(sender);
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

        protected virtual void ChildCompleted(object sender)
        {

        }

        protected void EachChild(Action<IInteraction> action, Func<IInteraction, bool> where = null)
        {
            var enumerable = _interactions;
            if (where != null)
                enumerable = enumerable.Where(where).ToList();

            foreach (var interaction in enumerable)
                action(interaction);
        }

        private void ChildActivated(object sender)
        {
            // disable all interactions except the one that sent an activation signal
            DisableChildren(i => i != sender);
        }

        private void ChildDeactivated()
        {
            // re-enable all interactions so that they can now handle events
            EnableChildren();
        }

        protected void EnableChildren(Func<IInteraction, bool> predicate = null)
        {
            EachChild(i => i.IsEnabled = true, predicate);
        }

        protected void DisableChildren(Func<IInteraction, bool> predicate = null)
        {
            EachChild(i => i.IsEnabled = false, predicate);
        }
    }
}