using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PetPulse
{
    public partial class SettingsWindow : Window
    {
        private bool _loading;

        public SettingsWindow(bool openRemarks = false)
        {
            InitializeComponent();
            LstLanguages.ItemsSource = Loc.LangNames;
            SyncToggles();
            RefreshDynamic();

            Loc.LanguageChanged += RefreshDynamic;
            Closed += (s, e) => Loc.LanguageChanged -= RefreshDynamic;

            if (openRemarks) RbRemarks.IsChecked = true;
        }

        private void SyncToggles()
        {
            _loading = true;
            TglLight.IsChecked = ThemeManager.IsLight;
            TglNotify.IsChecked = AppSettingsStore.Current.NotifyToday;
            TglConfirmExit.IsChecked = AppSettingsStore.Current.ConfirmExit;
            LstLanguages.SelectedIndex = Loc.Index;
            _loading = false;
        }

        private void RefreshDynamic()
        {
            bool dev = AppSettingsStore.Current.DeveloperMode;

            LblRemarksText.Text = Loc.Get(dev ? "remarks_dev_text" : "remarks_user_text");
            BtnRemarkAction.SetResourceReference(ContentControl.ContentProperty, dev ? "T_remark_view_btn" : "T_remark_add_btn");

            int open = RemarksStore.Items.Count(r => !r.Done);
            int done = RemarksStore.Items.Count(r => r.Done);
            LblRemarksInfo.Text = dev ? Loc.F("remark_counts", open, done) : "";

            LblRoleCurrent.Text = Loc.F("role_current", Loc.Get(dev ? "role_dev" : "role_user"));
            LblRoleDesc.Text = Loc.Get(dev ? "role_desc_dev" : "role_desc_user");
            BtnRole.SetResourceReference(ContentControl.ContentProperty, dev ? "T_role_switch_to_user" : "T_role_switch_to_dev");
        }

        private void Category_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is not RadioButton rb) return;
            string tag = rb.Tag?.ToString() ?? "";

            PawPlaceholder.Visibility = Visibility.Collapsed;
            PanelGraphics.Visibility = tag == "Graphics" ? Visibility.Visible : Visibility.Collapsed;
            PanelLanguage.Visibility = tag == "Language" ? Visibility.Visible : Visibility.Collapsed;
            PanelNotify.Visibility = tag == "Notify" ? Visibility.Visible : Visibility.Collapsed;
            PanelRemarks.Visibility = tag == "Remarks" ? Visibility.Visible : Visibility.Collapsed;
            PanelGeneral.Visibility = tag == "General" ? Visibility.Visible : Visibility.Collapsed;

            RefreshDynamic();
        }

        private void TglLight_Click(object sender, RoutedEventArgs e) => ThemeManager.Apply(TglLight.IsChecked == true);

        private void LstLanguages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_loading || LstLanguages.SelectedIndex < 0) return;
            Loc.Set(LstLanguages.SelectedIndex);
        }

        private void TglNotify_Click(object sender, RoutedEventArgs e)
        {
            AppSettingsStore.Current.NotifyToday = TglNotify.IsChecked == true;
            AppSettingsStore.Save();
        }

        private void TglConfirmExit_Click(object sender, RoutedEventArgs e)
        {
            AppSettingsStore.Current.ConfirmExit = TglConfirmExit.IsChecked == true;
            AppSettingsStore.Save();
        }

        private void BtnRemarkAction_Click(object sender, RoutedEventArgs e)
        {
            if (AppSettingsStore.Current.DeveloperMode)
            {
                new RemarksWindow { Owner = this }.ShowDialog();
                RefreshDynamic();
            }
            else
            {
                var window = new AddRemarkWindow { Owner = this };
                if (window.ShowDialog() == true)
                {
                    LblRemarksInfo.Text = "✔ " + Loc.Get("remark_saved");
                }
            }
        }

        private void BtnRole_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult answer = MessageBox.Show(Loc.Get("role_restart_msg"), Loc.Get("role_title"), MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (answer != MessageBoxResult.Yes) return;

            AppSettingsStore.Current.DeveloperMode = !AppSettingsStore.Current.DeveloperMode;
            AppSettingsStore.Save();
            AppRestarter.Restart("--remarks");
        }

        private void ResetSettings_Click(object sender, RoutedEventArgs e)
        {
            AppSettingsStore.Current.NotifyToday = true;
            AppSettingsStore.Current.ConfirmExit = true;
            ThemeManager.Apply(false);
            SyncToggles();
        }
    }
}