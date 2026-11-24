using System.Windows;

namespace PetPulse
{
    public partial class AddRemarkWindow : Window
    {
        public AddRemarkWindow()
        {
            InitializeComponent();
            TxtAuthor.Text = Loc.Get("role_user");
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string title = TxtTitle.Text.Trim();
            string details = TxtDetails.Text.Trim();
            string author = TxtAuthor.Text.Trim();

            if (title.Length == 0)
            {
                LblError.Text = "⚠ " + Loc.Get("rem_err_title");
                return;
            }
            if (details.Length < 10)
            {
                LblError.Text = "⚠ " + Loc.Get("rem_err_details");
                return;
            }

            RemarksStore.Add(title, details, author.Length == 0 ? Loc.Get("role_user") : author);
            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
    }
}