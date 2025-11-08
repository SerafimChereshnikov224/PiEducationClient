using PiServer.Services;
using PiServer.version_2.controllers;

namespace PiClientV1
{
    public partial class App : Application
    {
        public App()
        {
            // ГЛОБАЛЬНЫЙ ЛОВЕЦ ИСКЛЮЧЕНИЙ
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                var exception = e.ExceptionObject as Exception;
                System.Diagnostics.Debug.WriteLine($"🔴🔴🔴 GLOBAL UNHANDLED EXCEPTION 🔴🔴🔴");
                System.Diagnostics.Debug.WriteLine($"Message: {exception?.Message}");
                System.Diagnostics.Debug.WriteLine($"Type: {exception?.GetType().FullName}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {exception?.StackTrace}");

                if (exception?.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner Exception: {exception.InnerException.Message}");
                    System.Diagnostics.Debug.WriteLine($"Inner Stack Trace: {exception.InnerException.StackTrace}");
                }
            };

            // Ловец исключений тасков
            TaskScheduler.UnobservedTaskException += (sender, e) =>
            {
                System.Diagnostics.Debug.WriteLine($"🔴🔴🔴 UNOBSERVED TASK EXCEPTION 🔴🔴🔴");
                System.Diagnostics.Debug.WriteLine($"Message: {e.Exception.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {e.Exception.StackTrace}");
                e.SetObserved();
            };

            try
            {
                InitializeComponent();
                System.Diagnostics.Debug.WriteLine("Inited");
                TestPiServer();
                MainPage = new AppShell();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"🔴🔴🔴 APP INIT EXCEPTION 🔴🔴🔴");
                System.Diagnostics.Debug.WriteLine($"Message: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw;
            }
        }

        private void TestPiServer()
        {
            try
            {
                // Пробуем создать основной контроллер
                var controller = new PiProcessApi();
                System.Diagnostics.Debug.WriteLine("✅ PiProcessController создан");

                // Пробуем создать сервис  
                var services = new MachineBundle();
                System.Diagnostics.Debug.WriteLine("✅ MachineServices создан");

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error server");
            }
        }
    }
}