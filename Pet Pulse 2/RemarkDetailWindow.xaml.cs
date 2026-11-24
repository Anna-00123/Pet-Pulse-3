using System.Windows;

namespace PetPulse
{
    public partial class RemarkDetailWindow : Window
    {
        public RemarkDetailWindow(Remark remark)
        {
            InitializeComponent();

            LblTitle.Text = remark.Title;
            LblDetails.Text = remark.Details;
            LblMeta.Text = $"{Loc.Get("added")}: {remark.CreatedText}  ·  {remark.Author}";
            LblStatus.Text = remark.Status;
            LblStatus.Foreground = ThemeManager.GetBrush(remark.Done ? "TextMutedBrush" : "WarnBrush");
        }

        private void Close_Click(object sender, RoutedEventArgs e) => Close();
    }
}