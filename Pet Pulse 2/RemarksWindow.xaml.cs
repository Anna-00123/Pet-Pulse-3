using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PetPulse
{
    public partial class RemarksWindow : Window
    {
        public RemarksWindow()
        {
            InitializeComponent();
            LstRemarks.ItemsSource = RemarksStore.Items;
            UpdateCounts();
        }

        private void UpdateCounts()
        {
            int open = RemarksStore.Items.Count(r => !r.Done);
            int done = RemarksStore.Items.Count(r => r.Done);
            LblCounts.Text = Loc.F("remark_counts", open, done);
        }

        private void Done_Click(object sender, RoutedEventArgs e)
        {
            RemarksStore.Save();
            UpdateCounts();
        }

        private void Card_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.DataContext is Remark remark)
            {
                new RemarkDetailWindow(remark) { Owner = this }.ShowDialog();
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e) => Close();
    }
}