using System.Windows;
using System.Windows.Controls;

namespace PetPulse
{
    public partial class EmptyStateView : UserControl
    {
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(nameof(Icon), typeof(string), typeof(EmptyStateView), new PropertyMetadata("🐾"));

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(EmptyStateView), new PropertyMetadata(""));

        public static readonly DependencyProperty HintProperty =
            DependencyProperty.Register(nameof(Hint), typeof(string), typeof(EmptyStateView), new PropertyMetadata(""));

        public static readonly DependencyProperty ActionTextProperty =
            DependencyProperty.Register(nameof(ActionText), typeof(string), typeof(EmptyStateView),
                new PropertyMetadata("", OnActionTextChanged));

        public event RoutedEventHandler? ActionClick;

        public EmptyStateView()
        {
            InitializeComponent();
        }

        public string Icon
        {
            get => (string)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public string Hint
        {
            get => (string)GetValue(HintProperty);
            set => SetValue(HintProperty, value);
        }

        public string ActionText
        {
            get => (string)GetValue(ActionTextProperty);
            set => SetValue(ActionTextProperty, value);
        }

        private static void OnActionTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (EmptyStateView)d;
            view.ActionButton.Visibility = string.IsNullOrWhiteSpace(e.NewValue as string)
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        private void ActionButton_Click(object sender, RoutedEventArgs e)
        {
            ActionClick?.Invoke(this, e);
        }
    }
}