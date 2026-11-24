using System.Linq;
using System.Windows;

namespace PetPulse
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ShutdownMode = ShutdownMode.OnExplicitShutdown;

            ThemeManager.Init();
            Loc.Init();
            RemarksStore.Load();

            if (!e.Args.Contains("--remarks"))
            {
                new SplashWindow().ShowDialog();
            }

            var main = new MainWindow();
            MainWindow = main;
            ShutdownMode = ShutdownMode.OnMainWindowClose;
            main.Show();
        }
    }
}