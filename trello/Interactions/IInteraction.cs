using System;
using System.Windows;

namespace trello.Interactions
{
    public interface IInteraction
    {
        bool IsActive { get; }

        bool IsEnabled { get; set; }

        void Initialize();

        void AddElement(FrameworkElement element);

        void RemoveElement(FrameworkElement element);

        event EventHandler Activated;

        event EventHandler Deactivated;

        event EventHandler Completed;
    }
}