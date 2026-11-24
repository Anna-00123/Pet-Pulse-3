using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PetPulse
{
    public class ClinicItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Address { get; set; } = "";
        public double DistanceKm { get; set; }
        public double Rating { get; set; }
        public int Reviews { get; set; }

        public string RatingText => string.Format(Loc.Culture, "★ {0:0.0}", Rating);
        public string Meta => string.Format(Loc.Culture, "{0:0.0} {1} · {2} {3}", DistanceKm, Loc.Get("km"), Reviews, Loc.Get("reviews"));

        public static List<ClinicItem> CreateDemoList() => new List<ClinicItem>
        {
            new ClinicItem { Id = 1,  Name = "ВетКлініка «Добрий лікар»",       Address = "вул. Сумська, 24",            DistanceKm = 0.4, Rating = 4.9, Reviews = 212 },
            new ClinicItem { Id = 2,  Name = "Зооцентр «Лапа до лапи»",         Address = "просп. Науки, 18",            DistanceKm = 0.7, Rating = 4.8, Reviews = 187 },
            new ClinicItem { Id = 3,  Name = "Клініка «АйБоліт»",               Address = "вул. Пушкінська, 40",         DistanceKm = 1.1, Rating = 4.7, Reviews = 143 },
            new ClinicItem { Id = 4,  Name = "ВетЦентр «Пухнастик»",            Address = "вул. Клочківська, 9",         DistanceKm = 1.4, Rating = 4.6, Reviews = 96 },
            new ClinicItem { Id = 5,  Name = "Зоолікарня «24/7»",               Address = "просп. Незалежності, 33",     DistanceKm = 1.8, Rating = 4.9, Reviews = 301 },
            new ClinicItem { Id = 6,  Name = "Ветклініка «Хвостик»",            Address = "вул. Данилевського, 7",       DistanceKm = 2.2, Rating = 4.5, Reviews = 74 },
            new ClinicItem { Id = 7,  Name = "Клініка «ВетМайстер»",            Address = "вул. Гуданова, 15",           DistanceKm = 2.6, Rating = 4.7, Reviews = 128 },
            new ClinicItem { Id = 8,  Name = "Клініка «Здорова лапка»",         Address = "вул. Соборності, 21",         DistanceKm = 3.0, Rating = 4.4, Reviews = 59 },
            new ClinicItem { Id = 9,  Name = "Центр «Друг людини»",             Address = "просп. Героїв Харкова, 11",   DistanceKm = 3.5, Rating = 4.8, Reviews = 165 },
            new ClinicItem { Id = 10, Name = "Клініка «Котопес»",               Address = "вул. Академіка Павлова, 5",   DistanceKm = 4.1, Rating = 4.6, Reviews = 88 },
        };
    }

    public partial class VetBookingWindow : Window
    {
        private class SlotTag
        {
            public DateTime Time;
            public string? BusyKey;
        }

        private static readonly HashSet<string> BookedSlots = new HashSet<string>();

        private readonly Action<string, DateTime>? _onBooked;
        private ClinicItem? _clinic;
        private DateTime? _slot;
        private int _step = 1;

        public VetBookingWindow(Action<string, DateTime>? onBooked = null)
        {
            InitializeComponent();
            _onBooked = onBooked;

            LstClinics.ItemsSource = ClinicItem.CreateDemoList();

            for (int i = 0; i < 14; i++)
            {
                DateTime d = DateTime.Today.AddDays(i);
                string label = i == 0 ? Loc.F("day_today_fmt", d.ToString("dd.MM", Loc.Culture))
                             : i == 1 ? Loc.F("day_tomorrow_fmt", d.ToString("dd.MM", Loc.Culture))
                             : d.ToString("dddd, dd.MM", Loc.Culture);
                CmbDate.Items.Add(new ComboBoxItem { Content = label, Tag = d });
            }
            CmbDate.SelectedIndex = 0;

            GoStep(1);
        }

        private void GoStep(int step)
        {
            _step = step;
            PanelClinics.Visibility = step == 1 ? Visibility.Visible : Visibility.Collapsed;
            PanelDateTime.Visibility = step == 2 ? Visibility.Visible : Visibility.Collapsed;
            PanelPet.Visibility = step == 3 ? Visibility.Visible : Visibility.Collapsed;
            PanelDone.Visibility = step == 4 ? Visibility.Visible : Visibility.Collapsed;

            LblStep.Text = step switch
            {
                1 => Loc.Get("step1"),
                2 => Loc.Get("step2"),
                3 => Loc.Get("step3"),
                _ => Loc.Get("step_done")
            };

            BtnBack.Visibility = (step == 2 || step == 3) ? Visibility.Visible : Visibility.Collapsed;
            BtnNext.Visibility = step == 1 ? Visibility.Collapsed : Visibility.Visible;
            BtnNext.Content = step switch { 2 => Loc.Get("next"), 3 => Loc.Get("book_btn"), _ => Loc.Get("close") };
            BtnNext.IsEnabled = step != 2 || _slot != null;

            if (step == 1) LstClinics.SelectedItem = null;
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e) => GoStep(_step == 3 ? 2 : 1);

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            switch (_step)
            {
                case 2: GoStep(3); break;
                case 3: Submit(); break;
                default: Close(); break;
            }
        }

        private void LstClinics_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LstClinics.SelectedItem is not ClinicItem c) return;

            if (_clinic?.Id != c.Id) _slot = null;
            _clinic = c;

            LblClinicName.Text = c.Name;
            LblClinicInfo.Text = $"{c.Address}  ·  {c.RatingText}  ·  {c.Meta}";
            BuildSlots();
            GoStep(2);
        }

        private void CmbDate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_clinic == null) return;
            _slot = null;
            BtnNext.IsEnabled = false;
            BuildSlots();
        }

        private static string Key(ClinicItem c, DateTime t) => $"{c.Id}|{t:yyyyMMddHHmm}";

        private void BuildSlots()
        {
            if (_clinic == null || CmbDate.SelectedItem is not ComboBoxItem { Tag: DateTime date }) return;

            SlotsPanel.Children.Clear();
            LblSlotInfo.Text = "";

            var rnd = new Random(_clinic.Id * 1000 + date.DayOfYear);

            for (int hour = 9; hour < 19; hour++)
            {
                foreach (int minute in new[] { 0, 30 })
                {
                    DateTime t = date.AddHours(hour).AddMinutes(minute);
                    bool randomBusy = rnd.NextDouble() < 0.35;

                    string? busyKey = null;
                    if (t <= DateTime.Now) busyKey = "slot_past";
                    else if (randomBusy || BookedSlots.Contains(Key(_clinic, t))) busyKey = "slot_busy";

                    var btn = new Button
                    {
                        Content = t.ToString("HH:mm"),
                        Width = 84,
                        Height = 38,
                        Margin = new Thickness(0, 0, 10, 10),
                        Tag = new SlotTag { Time = t, BusyKey = busyKey }
                    };
                    btn.Click += Slot_Click;
                    SlotsPanel.Children.Add(btn);
                }
            }
            RestyleSlots();
        }

        private void RestyleSlots()
        {
            foreach (Button b in SlotsPanel.Children.OfType<Button>())
            {
                if (b.Tag is not SlotTag tag) continue;
                string style = tag.BusyKey != null ? "SlotBusy"
                             : (_slot.HasValue && tag.Time == _slot.Value) ? "SlotSelected" : "SlotFree";
                b.Style = (Style)FindResource(style);
            }
        }

        private void Slot_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button b || b.Tag is not SlotTag tag) return;

            if (tag.BusyKey != null)
            {
                LblSlotInfo.Foreground = ThemeManager.GetBrush("AlertBrush");
                LblSlotInfo.Text = $"⛔ {tag.Time:HH:mm} — {Loc.Get(tag.BusyKey)}. {Loc.Get("slot_pick_other")}";
                return;
            }

            _slot = tag.Time;
            LblSlotInfo.Foreground = ThemeManager.GetBrush("AccentMintBrush");
            LblSlotInfo.Text = "✔ " + Loc.F("slot_chosen", tag.Time.ToString("d", Loc.Culture), tag.Time.ToString("HH:mm"));
            BtnNext.IsEnabled = true;
            RestyleSlots();
        }

        private void TxtPhone_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(ch => char.IsDigit(ch) || "+-() ".IndexOf(ch) >= 0);
        }

        private void Submit()
        {
            string pet = TxtPetName.Text.Trim();
            string owner = TxtOwner.Text.Trim();
            string phone = TxtPhone.Text.Trim();
            string complaint = TxtComplaint.Text.Trim();
            int digits = phone.Count(char.IsDigit);

            string? error =
                pet.Length == 0 ? Loc.Get("err_pet") :
                (RbCat.IsChecked != true && RbDog.IsChecked != true) ? Loc.Get("err_species") :
                owner.Length == 0 ? Loc.Get("err_owner") :
                (digits < 10 || digits > 13) ? Loc.Get("err_phone") : null;

            if (error != null)
            {
                LblError.Text = "⚠ " + error;
                return;
            }
            LblError.Text = "";

            if (_clinic == null || _slot == null) return;

            if (BookedSlots.Contains(Key(_clinic, _slot.Value)))
            {
                MessageBox.Show(Loc.Get("slot_taken_msg"), Loc.Get("vet_title"), MessageBoxButton.OK, MessageBoxImage.Warning);
                GoStep(2);
                BuildSlots();
                return;
            }

            BookedSlots.Add(Key(_clinic, _slot.Value));

            string species = Loc.Get(RbCat.IsChecked == true ? "cat" : "dog");
            string when = $"{_slot.Value.ToString("d", Loc.Culture)} {_slot.Value:HH:mm}";
            LblDone.Text =
                $"{Loc.Get("d_clinic")}: {_clinic.Name}\n" +
                $"{Loc.Get("d_address")}: {_clinic.Address}\n" +
                $"{Loc.Get("d_datetime")}: {when}\n" +
                $"{Loc.Get("d_pet")}: {pet} ({species})\n" +
                $"{Loc.Get("d_owner")}: {owner}, {Loc.Get("d_phone_short")} {phone}\n" +
                $"{Loc.Get("d_complaint")}: {(complaint.Length == 0 ? Loc.Get("not_given") : complaint)}";

            _onBooked?.Invoke(_clinic.Name, _slot.Value);
            GoStep(4);
        }
    }
}