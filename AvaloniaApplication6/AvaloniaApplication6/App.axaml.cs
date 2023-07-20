using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Platform;
using Avalonia.Markup.Xaml;

namespace AvaloniaApplication6
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override async void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow();

                MainWindow window = (MainWindow)desktop.MainWindow;
                window._allProcesses = await window._serviceWorsk.GetProcesses();
                window.AddOutputProcesses(window._allProcesses);
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}