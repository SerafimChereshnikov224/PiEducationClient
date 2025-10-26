using Microsoft.UI.Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PiClientV1.WinUI
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : MauiWinUIApplication
    {
        public App()
        {
            // Добавляем обработчик ДО InitializeComponent
            this.UnhandledException += (sender, e) =>
            {
                System.Diagnostics.Debug.WriteLine($"🔴🔴🔴 WINDOWS UNHANDLED EXCEPTION 🔴🔴🔴");
                System.Diagnostics.Debug.WriteLine($"Message: {e.Message}");
                System.Diagnostics.Debug.WriteLine($"Exception Type: {e.Exception.GetType()}");
                System.Diagnostics.Debug.WriteLine($"Exception: {e.Exception}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {e.Exception.StackTrace}");

                if (e.Exception.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner Exception: {e.Exception.InnerException}");
                    System.Diagnostics.Debug.WriteLine($"Inner Stack Trace: {e.Exception.InnerException.StackTrace}");
                }
            };

            this.InitializeComponent();
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }

}
