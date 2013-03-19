using System;
using System.Windows;

namespace trello.Interactions
{
    public abstract class InteractionBase : IInteraction
    {
        private bool _isActive;

        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;

                if (_isActive)
                {
                    if (Activated != null)
                        Activated(this, EventArgs.Empty);
                }
                else
                {
                    if (Deactivated != null)
                        Deactivated(this, EventArgs.Empty);
                }
            }
        }

        public bool IsEnabled { get; set; }

        public event EventHandler Activated;

        public event EventHandler Deactivated;

        public virtual void Initialize()
        {
            
        }

        public virtual void AddElement(FrameworkElement element)
        {
            
        }

        protected void RefreshView()
        {
            
        }
    }
}