using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PetPulse
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<MedicineItem> Medicines { get; set; }
        public ObservableCollection<ReminderItem> Reminders { get; set; }

        private static readonly (int NormMin, int NormMax, int WarnMin, int AttnMin)[] PulseRanges =
        {
            (90, 140, 70, 50),
            (70, 120, 55, 40),
            (60, 100, 50, 40)
        };

        public MainWindow()
        {
            InitializeComponent();
            ThemeManager.ThemeChanged += OnThemeChanged;
            Loc.LanguageChanged += ApplyLanguage;

            var med1 = new MedicineItem { NameKey = "seed_med1_name", DosageKey = "seed_med1_dose", DayKey = "today" };
            var med2 = new MedicineItem { NameKey = "seed_med2_name", DosageKey = "seed_med2_dose", DayKey = "tomorrow" };

            Medicines = new ObservableCollection<MedicineItem> { med1, med2 };

            Reminders = new ObservableCollection<ReminderItem>
            {
                new ReminderItem { Kind = ReminderKind.Medicine, Medicine = med1, DayKey = "today" },
                new ReminderItem { Kind = ReminderKind.Medicine, Medicine = med2, DayKey = "tomorrow" },
                new ReminderItem { Kind = ReminderKind.VisitPlanned, DayKey = "tomorrow", IsEnabled = false }
            };

            LstMedicines.ItemsSource = Medicines;
            LstReminders.ItemsSource = Reminders;

            Medicines.CollectionChanged += (s, e) => UpdateEmptyStates();
            Reminders.CollectionChanged += (s, e) => UpdateEmptyStates();
            EmptyMeds.ActionClick += (s, e) => FocusInput(TxtMedName);
            EmptyReminders.ActionClick += (s, e) => FocusInput(TxtCustomReminder);
            SetActiveNav(NavMeds);

            ApplyLanguage();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (Environment.GetCommandLineArgs().Contains("--remarks"))
            {
                new SettingsWindow(true) { Owner = this }.ShowDialog();
            }
        }

        private void ApplyLanguage()
        {
            if (Medicines == null || Reminders == null) return;

            SetIfDefault(TxtMedName, "med_name_ph");
            SetIfDefault(TxtMedDose, "med_dose_default");
            SetIfDefault(TxtCustomReminder, "rem_ph");
            SetIfDefault(TxtDocChip, "doc_chip");
            SetIfDefault(TxtDocValid, "doc_valid");

            foreach (var m in Medicines) m.Refresh();
            foreach (var r in Reminders) r.Refresh();

            RefreshCombo(CmbMedDay);
            RefreshCombo(CmbReminderDay);
            RefreshCombo(CmbDogBreedSize);

            UpdateLab3Texts();
            UpdateHealthMetrics();
        }

        private static void SetIfDefault(TextBox box, string key)
        {
            if (box.Text.Length == 0 || Loc.IsAny(key, box.Text)) box.Text = Loc.Get(key);
        }

        private static void RefreshCombo(ComboBox combo)
        {
            int i = combo.SelectedIndex;
            if (i < 0) return;
            combo.SelectedIndex = -1;
            combo.SelectedIndex = i;
        }

        private void BtnMenu_Click(object sender, RoutedEventArgs e) => MenuPopup.IsOpen = true;

        private void MenuSettings_Click(object sender, RoutedEventArgs e)
        {
            MenuPopup.IsOpen = false;
            new SettingsWindow { Owner = this }.ShowDialog();
        }

        private void MenuVet_Click(object sender, RoutedEventArgs e)
        {
            MenuPopup.IsOpen = false;
            new VetBookingWindow(OnVisitBooked) { Owner = this }.ShowDialog();
        }

        private void OnVisitBooked(string clinic, DateTime when)
        {
            string dayKey = when.Date == DateTime.Today ? "today"
                          : when.Date == DateTime.Today.AddDays(1) ? "tomorrow" : "date";

            Reminders.Add(new ReminderItem
            {
                Kind = ReminderKind.VisitBooked,
                Text = clinic,
                Time = when.ToString("HH:mm"),
                DayKey = dayKey,
                DateText = when.ToString("d", Loc.Culture),
                IsEnabled = true
            });
        }

        private void OnThemeChanged()
        {
            if (Reminders != null)
            {
                foreach (var r in Reminders) r.RefreshBrushes();
            }
            UpdateHealthMetrics();
            UpdateQuickThemeText();
        }

        private void TxtMedName_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Loc.IsAny("med_name_ph", TxtMedName.Text)) TxtMedName.Text = "";
        }

        private void TxtMedName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtMedName.Text)) TxtMedName.Text = Loc.Get("med_name_ph");
        }

        private void TxtCustomReminder_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Loc.IsAny("rem_ph", TxtCustomReminder.Text)) TxtCustomReminder.Text = "";
        }

        private void TxtCustomReminder_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtCustomReminder.Text)) TxtCustomReminder.Text = Loc.Get("rem_ph");
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            if (AppSettingsStore.Current.ConfirmExit)
            {
                MessageBoxResult result = MessageBox.Show(Loc.Get("exit_confirm"), Loc.Get("exit"), MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes) return;
            }
            Application.Current.Shutdown();
        }

        private void ResetHealthDefaults_Click(object sender, RoutedEventArgs e)
        {
            TxtTemp.Text = "38.5";

            switch (CmbDogBreedSize.SelectedIndex)
            {
                case 0:
                    TxtWeight.Text = "3.5";
                    TxtPulse.Text = "100";
                    break;
                case 1:
                    TxtWeight.Text = "10.0";
                    TxtPulse.Text = "85";
                    break;
                case 2:
                    TxtWeight.Text = "25.0";
                    TxtPulse.Text = "75";
                    break;
                default:
                    TxtWeight.Text = "10.0";
                    TxtPulse.Text = "85";
                    break;
            }

            UpdateHealthMetrics();
        }

        private void TxtPulse_TextChanged(object sender, TextChangedEventArgs e) => UpdatePulseLogic();
        private void TxtWeight_TextChanged(object sender, TextChangedEventArgs e) => UpdateWeightLogic();
        private void TxtTemp_TextChanged(object sender, TextChangedEventArgs e) => UpdateTempLogic();
        private void CmbDogBreedSize_SelectionChanged(object sender, SelectionChangedEventArgs e) => UpdateHealthMetrics();

        private void UpdateHealthMetrics()
        {
            UpdatePulseLogic();
            UpdateWeightLogic();
            UpdateTempLogic();
        }

        private static void SetStatus(TextBlock label, TextBox box, string text, string brushKey)
        {
            Brush brush = ThemeManager.GetBrush(brushKey);
            label.Text = text;
            label.Foreground = brush;
            box.Foreground = brush;
        }

        private const string Ok = "AccentMintBrush";
        private const string Warn = "WarnBrush";
        private const string Alert = "AlertBrush";

        private void UpdatePulseLogic()
        {
            if (TxtPulse == null || LblPulseStatus == null || CmbDogBreedSize == null) return;
            if (!int.TryParse(TxtPulse.Text, out int pulse)) return;

            int idx = CmbDogBreedSize.SelectedIndex is 0 or 1 ? CmbDogBreedSize.SelectedIndex : 2;
            var r = PulseRanges[idx];

            if (pulse >= r.NormMin && pulse <= r.NormMax)
                SetStatus(LblPulseStatus, TxtPulse, Loc.F("pulse_norm", r.NormMin, r.NormMax), Ok);
            else if (pulse >= r.WarnMin && pulse < r.NormMin)
                SetStatus(LblPulseStatus, TxtPulse, Loc.Get("pulse_low"), Warn);
            else if (pulse >= r.AttnMin && pulse < r.WarnMin)
                SetStatus(LblPulseStatus, TxtPulse, Loc.Get("pulse_lower"), Alert);
            else if (pulse < r.AttnMin)
                SetStatus(LblPulseStatus, TxtPulse, Loc.Get("pulse_vlow"), Alert);
            else
                SetStatus(LblPulseStatus, TxtPulse, Loc.Get("pulse_high"), Alert);
        }

        private void UpdateWeightLogic()
        {
            if (TxtWeight == null || LblWeightStatus == null || CmbDogBreedSize == null) return;

            string cleanText = TxtWeight.Text.Replace(',', '.');
            if (!double.TryParse(cleanText, NumberStyles.Any, CultureInfo.InvariantCulture, out double weight)) return;

            if (CmbDogBreedSize.SelectedIndex == 2)
            {
                if (weight > 40.0)
                    SetStatus(LblWeightStatus, TxtWeight, Loc.Get("w_obese"), Alert);
                else if (weight >= 15.0)
                    SetStatus(LblWeightStatus, TxtWeight, Loc.Get("w_ok"), Ok);
                else if (weight >= 10.0)
                    SetStatus(LblWeightStatus, TxtWeight, Loc.Get("w_def_warn"), Warn);
                else
                    SetStatus(LblWeightStatus, TxtWeight, Loc.Get("w_crit"), Alert);
            }
            else
            {
                double minWeight = CmbDogBreedSize.SelectedIndex == 0 ? 1.0 : 5.0;
                double maxWeight = CmbDogBreedSize.SelectedIndex == 0 ? 5.0 : 15.0;

                if (weight > maxWeight)
                    SetStatus(LblWeightStatus, TxtWeight, Loc.Get("w_obese"), Alert);
                else if (weight < minWeight)
                    SetStatus(LblWeightStatus, TxtWeight, Loc.Get("w_def_low"), Warn);
                else
                    SetStatus(LblWeightStatus, TxtWeight, Loc.Get("w_ok"), Ok);
            }
        }

        private void UpdateTempLogic()
        {
            if (TxtTemp == null || LblTempStatus == null) return;

            string cleanText = TxtTemp.Text.Replace(',', '.');
            if (!double.TryParse(cleanText, NumberStyles.Any, CultureInfo.InvariantCulture, out double temp)) return;

            if (temp >= 37.5 && temp <= 39.0)
                SetStatus(LblTempStatus, TxtTemp, Loc.Get("t_norm"), Ok);
            else if (temp >= 34.0 && temp < 37.5)
                SetStatus(LblTempStatus, TxtTemp, Loc.Get("t_low"), Warn);
            else
                SetStatus(LblTempStatus, TxtTemp, Loc.Get("t_crit"), Alert);
        }

        private void AddMed_Click(object sender, RoutedEventArgs e)
        {
            string name = TxtMedName.Text.Trim();
            if (name.Length == 0 || Loc.IsAny("med_name_ph", name)) return;

            string dayKey = (CmbMedDay.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "today";

            var med = new MedicineItem
            {
                CustomName = name,
                CustomDosage = TxtMedDose.Text.Trim(),
                DayKey = dayKey
            };
            Medicines.Add(med);

            Reminders.Add(new ReminderItem { Kind = ReminderKind.Medicine, Medicine = med, DayKey = dayKey, IsEnabled = true });

            if (dayKey == "today" && AppSettingsStore.Current.NotifyToday)
            {
                MessageBox.Show(Loc.F("notify_text", name), Loc.Get("notify_title"), MessageBoxButton.OK, MessageBoxImage.Information);
            }

            TxtMedName.Text = Loc.Get("med_name_ph");
        }

        private void DeleteMed_Click(object sender, RoutedEventArgs e)
        {
            if (LstMedicines.SelectedItem is MedicineItem selectedMed)
            {
                var matching = Reminders.Where(r => r.Medicine == selectedMed).ToList();
                foreach (var reminder in matching)
                {
                    Reminders.Remove(reminder);
                }
                Medicines.Remove(selectedMed);
            }
        }

        private void AddCustomReminder_Click(object sender, RoutedEventArgs e)
        {
            string text = TxtCustomReminder.Text.Trim();
            if (text.Length == 0 || Loc.IsAny("rem_ph", text)) return;

            string dayKey = (CmbReminderDay.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "today";

            Reminders.Add(new ReminderItem { Kind = ReminderKind.Custom, Text = text, DayKey = dayKey, IsEnabled = true });

            TxtCustomReminder.Text = Loc.Get("rem_ph");
        }

        private void DeleteReminder_Click(object sender, RoutedEventArgs e)
        {
            if (LstReminders.SelectedItem is ReminderItem selectedReminder)
            {
                Reminders.Remove(selectedReminder);
            }
        }

        private void ToggleReminder_Click(object sender, RoutedEventArgs e) => LstReminders.Items.Refresh();

        private void NavButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.Tag == null) return;

            SetActiveNav(btn);

            string tag = btn.Tag.ToString() ?? string.Empty;

            PanelMeds.Visibility = Visibility.Collapsed;
            PanelDocs.Visibility = Visibility.Collapsed;
            PanelReminders.Visibility = Visibility.Collapsed;
            PanelHealth.Visibility = Visibility.Collapsed;
            PanelAbout.Visibility = Visibility.Collapsed;

            switch (tag)
            {
                case "Meds": PanelMeds.Visibility = Visibility.Visible; break;
                case "Docs": PanelDocs.Visibility = Visibility.Visible; break;
                case "Reminders": PanelReminders.Visibility = Visibility.Visible; break;
                case "Health": PanelHealth.Visibility = Visibility.Visible; break;
                case "About": PanelAbout.Visibility = Visibility.Visible; break;
            }
        }

        private void MenuTheme_Click(object sender, RoutedEventArgs e)
        {
            MenuPopup.IsOpen = false;
            ThemeManager.Apply(!ThemeManager.IsLight);
        }

        private static string L(string uk, string ru, string en)
        {
            return Loc.Culture.TwoLetterISOLanguageName switch
            {
                "uk" => uk,
                "ru" => ru,
                _ => en
            };
        }

        private void UpdateLab3Texts()
        {
            EmptyMeds.Icon = "💊";
            EmptyMeds.Title = L("Список ліків порожній", "Список лекарств пуст", "No medicines yet");
            EmptyMeds.Hint = L("Додайте перший препарат: назву, дозування та день прийому",
                               "Добавьте первый препарат: название, дозировку и день приёма",
                               "Add the first medicine: name, dosage and day");
            EmptyMeds.ActionText = L("Додати ліки", "Добавить лекарство", "Add medicine");

            EmptyReminders.Icon = "🔔";
            EmptyReminders.Title = L("Нагадувань поки немає", "Напоминаний пока нет", "No reminders yet");
            EmptyReminders.Hint = L("Створіть нагадування або запишіться до ветеринара — воно з'явиться тут автоматично",
                                    "Создайте напоминание или запишитесь к ветеринару — оно появится здесь автоматически",
                                    "Create a reminder or book a vet visit — it will appear here automatically");
            EmptyReminders.ActionText = L("Створити нагадування", "Создать напоминание", "Create reminder");

            UpdateQuickThemeText();
            UpdateEmptyStates();
        }

        private void UpdateQuickThemeText()
        {
            if (BtnQuickTheme == null) return;
            BtnQuickTheme.Content = ThemeManager.IsLight
                ? "🌙  " + L("Темна тема", "Тёмная тема", "Dark theme")
                : "☀  " + L("Світла тема", "Светлая тема", "Light theme");
        }

        private void UpdateEmptyStates()
        {
            if (EmptyMeds == null || EmptyReminders == null || Medicines == null || Reminders == null) return;
            EmptyMeds.Visibility = Medicines.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            EmptyReminders.Visibility = Reminders.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private static void FocusInput(TextBox box)
        {
            box.Focus();
            box.SelectAll();
        }

        private void SetActiveNav(Button active)
        {
            foreach (var b in new[] { NavMeds, NavDocs, NavReminders, NavHealth, NavAbout })
            {
                if (b == active)
                {
                    b.SetResourceReference(BackgroundProperty, "AccentEmeraldBrush");
                    b.Foreground = Brushes.White;
                    b.FontWeight = FontWeights.Bold;
                }
                else
                {
                    b.ClearValue(BackgroundProperty);
                    b.Foreground = Brushes.Black;
                    b.FontWeight = FontWeights.SemiBold;
                }
            }
        }
    }

    public class MedicineItem : INotifyPropertyChanged
    {
        public string? NameKey { get; set; }
        public string? DosageKey { get; set; }
        public string CustomName { get; set; } = string.Empty;
        public string CustomDosage { get; set; } = string.Empty;
        public string DayKey { get; set; } = "today";

        public string Name => NameKey != null ? Loc.Get(NameKey) : CustomName;
        public string Dosage => DosageKey != null ? Loc.Get(DosageKey) : CustomDosage;
        public string ScheduleDay => Loc.Get(DayKey);

        public void Refresh() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));

        public event PropertyChangedEventHandler? PropertyChanged;
    }

    public enum ReminderKind { Medicine, VisitPlanned, VisitBooked, Custom }

    public class ReminderItem : INotifyPropertyChanged
    {
        private bool _isEnabled = true;

        public ReminderKind Kind { get; set; }
        public MedicineItem? Medicine { get; set; }
        public string Text { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
        public string DayKey { get; set; } = "today";
        public string DateText { get; set; } = string.Empty;

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                OnPropertyChanged(nameof(IsEnabled));
                OnPropertyChanged(nameof(TextColorBrush));
                OnPropertyChanged(nameof(SubTextColorBrush));
                OnPropertyChanged(nameof(BadgeVisibility));
            }
        }

        public string Title => Kind switch
        {
            ReminderKind.Medicine => "💊 " + Loc.F("r_med", Medicine?.Name ?? "", Medicine?.Dosage ?? ""),
            ReminderKind.VisitPlanned => "🐾 " + Loc.Get("r_visit_planned"),
            ReminderKind.VisitBooked => "🐾 " + Loc.F("r_visit_booked", Text, Time),
            _ => Text
        };

        public string DayText => DayKey == "date" ? DateText : Loc.Get(DayKey);
        public bool IsToday => DayKey == "today";
        public string ScheduleText => IsToday ? Loc.Get("sched_today") : Loc.F("sched_on", DayText.ToLower(Loc.Culture));
        public Visibility BadgeVisibility => (IsToday && IsEnabled) ? Visibility.Visible : Visibility.Collapsed;

        public Brush TextColorBrush
        {
            get
            {
                if (!IsEnabled) return ThemeManager.GetBrush("TextDisabledBrush");
                return IsToday ? ThemeManager.GetBrush("AccentMintBrush") : ThemeManager.GetBrush("TextMutedBrush");
            }
        }

        public Brush SubTextColorBrush
        {
            get
            {
                if (!IsEnabled) return ThemeManager.GetBrush("TextDisabledBrush");
                return IsToday ? ThemeManager.GetBrush("TextSecondaryBrush") : ThemeManager.GetBrush("TextMutedBrush");
            }
        }

        public void Refresh() => OnPropertyChanged(string.Empty);

        public void RefreshBrushes()
        {
            OnPropertyChanged(nameof(TextColorBrush));
            OnPropertyChanged(nameof(SubTextColorBrush));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}