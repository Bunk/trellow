using System.Windows;
using System.Windows.Controls;

namespace trello.Views.Controls
{
    public partial class DropShadowTextBlock : UserControl
    {
        public DropShadowTextBlock()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof (string), typeof (DropShadowTextBlock),
                                        new PropertyMetadata(TextPropertyChanged));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        private static void TextPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var ctrl = sender as DropShadowTextBlock;
            if (ctrl == null)
                return;

            ctrl.tb1.Text = args.NewValue.ToString();
        }
    }
}
