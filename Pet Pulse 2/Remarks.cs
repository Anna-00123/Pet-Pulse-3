using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;

namespace PetPulse
{
    public class Remark : INotifyPropertyChanged
    {
        private bool _done;

        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Details { get; set; } = "";
        public string Author { get; set; } = "";
        public DateTime Created { get; set; } = DateTime.Now;

        public bool Done
        {
            get => _done;
            set
            {
                if (_done == value) return;
                _done = value;
                OnChanged(nameof(Done));
                OnChanged(nameof(Status));
            }
        }

        [JsonIgnore]
        public string Summary
        {
            get
            {
                string one = Details.Replace("\r", " ").Replace("\n", " ").Trim();
                return one.Length <= 70 ? one : one.Substring(0, 70).TrimEnd() + "…";
            }
        }

        [JsonIgnore]
        public string CreatedText => Created.ToString("d", Loc.Culture);

        [JsonIgnore]
        public string Status => Loc.Get(Done ? "status_done" : "status_open");

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public static class RemarksStore
    {
        private static readonly string FilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PetPulse", "remarks.json");

        public static ObservableCollection<Remark> Items { get; private set; } = new ObservableCollection<Remark>();

        public static void Load()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    var list = JsonSerializer.Deserialize<Remark[]>(File.ReadAllText(FilePath));
                    if (list != null && list.Length > 0)
                    {
                        Items = new ObservableCollection<Remark>(list);
                        return;
                    }
                }
            }
            catch { }

            Items = new ObservableCollection<Remark>(Seed());
            Save();
        }

        public static void Save()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
                File.WriteAllText(FilePath, JsonSerializer.Serialize(Items.ToArray(), new JsonSerializerOptions { WriteIndented = true }));
            }
            catch { }
        }

        public static void Add(string title, string details, string author)
        {
            int id = Items.Count == 0 ? 1 : Items.Max(r => r.Id) + 1;
            Items.Add(new Remark { Id = id, Title = title, Details = details, Author = author, Created = DateTime.Now });
            Save();
        }

        private static Remark[] Seed() => new[]
        {
            new Remark { Id = 1, Done = true, Author = "Віктор М. (персона)", Created = DateTime.Today.AddDays(-6),
                Title = "Додати світлу тему",
                Details = "Темний інтерфейс втомлює очі, особливо вранці й за великим монітором. Потрібен перемикач світлої теми з високим контрастом тексту, а вибір має запам'ятовуватися між запусками." },
            new Remark { Id = 2, Done = true, Author = "Марина К. (персона)", Created = DateTime.Today.AddDays(-5),
                Title = "Запис до ветеринара без дзвінків",
                Details = "Зараз клініку доводиться шукати окремо й телефонувати. Хочеться обрати клініку зі списку, побачити вільний час, а потім внести дані тварини й отримати нагадування про візит." },
            new Remark { Id = 3, Author = "Марина К. (персона)", Created = DateTime.Today.AddDays(-4),
                Title = "Зберігати ліки й нагадування між запусками",
                Details = "Після закриття застосунку додані ліки та нагадування зникають. Потрібно зберігати їх у файлі й завантажувати під час запуску, щоб не вводити дані щоразу." },
            new Remark { Id = 4, Author = "Марина К. (персона)", Created = DateTime.Today.AddDays(-3),
                Title = "Системні сповіщення поза вікном застосунку",
                Details = "Нагадування видно лише тоді, коли відкрито PetPulse. Потрібні сповіщення Windows у потрібний час, навіть якщо застосунок згорнуто." },
            new Remark { Id = 5, Author = "Марина К. (персона)", Created = DateTime.Today.AddDays(-3),
                Title = "Експорт історії здоров'я у PDF",
                Details = "Перед візитом до ветеринара хочеться швидко показати історію ліків, вагу та пульс. Потрібна кнопка експорту в PDF з датами й показниками." },
            new Remark { Id = 6, Author = "Олексій Б. (персона)", Created = DateTime.Today.AddDays(-2),
                Title = "Профіль улюбленця",
                Details = "Не вистачає окремого профілю: кличка, вік, порода, вага й фото. Від профілю можна було б автоматично брати категорію собаки для перевірки показників." },
            new Remark { Id = 7, Author = "Віктор М. (персона)", Created = DateTime.Today.AddDays(-1),
                Title = "Збільшити шрифт і масштаб інтерфейсу",
                Details = "Дрібний текст у списках і підписах складно читати. Потрібне налаштування розміру тексту або масштабу всього інтерфейсу." },
        };
    }

    public static class AppRestarter
    {
        public static void Restart(string args)
        {
            string? exe = Environment.ProcessPath;
            if (exe != null)
            {
                Process.Start(new ProcessStartInfo(exe, args) { UseShellExecute = true });
            }
            Application.Current.Shutdown();
        }
    }
}