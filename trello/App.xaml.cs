using System.Windows;
using BugSense;

namespace trello
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            BugSenseHandler.Instance.Init(this, "8b22f4e5");
        }
    }
}