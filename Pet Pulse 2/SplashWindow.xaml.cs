using System.Threading.Tasks;
using System.Windows;

namespace PetPulse
{
    public partial class SplashWindow : Window
    {
        public SplashWindow()
        {
            InitializeComponent();
            Loaded += OnSplashLoaded;
        }

        private async void OnSplashLoaded(object sender, RoutedEventArgs e)
        {
            await InitializeAppAsync();
            Close();
        }

        private async Task InitializeAppAsync()
        {
            string[] stages = new string[]
            {
                "Завантаження модулів нагадувань...",
                "Зчитування профілю тварини...",
                "Перевірка розкладу ліків...",
                "Готово!"
            };

            for (int i = 0; i < stages.Length; i++)
            {
                StatusTextBlock.Text = stages[i];
                SplashProgressBar.Value = (i + 1) * 25;

                await Task.Delay(500);
            }
        }
    }
}