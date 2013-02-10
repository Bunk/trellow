using System.Windows;
using System.Windows.Controls;

namespace trello.Views
{
    public partial class EditableTextBlock : UserControl
    {
        public EditableTextBlock()
        {
            InitializeComponent();

            TextBlock.DataContext = this;
            TextBox.DataContext = this;

            MouseLeftButtonDown += (sender, args) => IsEditing = true;
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(EditableTextBlock), null);

        public string Text
        {
            get { return GetValue(TextProperty).ToString(); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty IsEditingProperty =
            DependencyProperty.Register("IsEditing", typeof(bool), typeof(EditableTextBlock),
            new PropertyMetadata(false, IsEditingPropertyChanged));

        public bool IsEditing
        {
            get { return (bool)GetValue(IsEditingProperty); }
            set { SetValue(IsEditingProperty, value); }
        }

        public static void IsEditingPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var s = sender as EditableTextBlock;
            if (s != null && (bool)e.NewValue)
            {
                s.Dispatcher.BeginInvoke(() =>
                {
                    s.TextBox.Focus();
                    s.TextBox.SelectAll();
                });

            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            IsEditing = false;
        }
    }
}
