namespace trello.Interactions
{
    /// <summary>
    /// Manages a composition of interactions by allowing child interactions to remain
    /// disabled (not listening to events) until certain conditions are met first.
    /// </summary>
    public abstract class CompositeInteractionManager : InteractionManager
    {
        public override void AddInteraction(IInteraction interaction)
        {
            base.AddInteraction(interaction);

            // child interactions on a composite interaction start out disabled
            // the composite will enable interaction when its interaction is triggered
            interaction.IsEnabled = false;
            interaction.Completed += (sender, args) => ChildCompleted(sender);
        }

        protected override void ChildActivated(object sender)
        {
            // When a child is activated, disable the other children
            EachChild(i => i.IsEnabled = false, i => i != sender);
        }

        protected override void ChildDeactivated()
        {
            // When a child is deactivated, disable all other children
            EachChild(i => i.IsEnabled = false);
        }

        protected virtual void ChildCompleted(object sender)
        {
            // When a child completes, we want to perform any cleanup necessary
            // (ie, animations, disables, etc)
            FinalizeInteraction();
        }

        protected void EnableChildInteractions()
        {
            EachChild(i => i.IsEnabled = true);
        }

        protected void DisableChildInteractions()
        {
            EachChild(i => i.IsEnabled = false);
        }

        protected virtual void FinalizeInteraction()
        {
            DisableChildInteractions();
        }
    }
}
