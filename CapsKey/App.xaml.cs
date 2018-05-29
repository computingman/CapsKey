using CapsKey.Helpers;
using CapsKey.Model;
using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace CapsKey
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ILog _log;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            XmlConfigurator.Configure();
            _log = LogManager.GetLogger(typeof(App));

            _log.Info($"Started CapsKey version {GetAppVersion()}.");

            ShutdownMode = ShutdownMode.OnMainWindowClose;

            var keyboardHook = new GlobalKeyboardHook();

            var mainModel = new MainModel();
            var mainWindow = new MainWindow();

            var settingsModel = new SettingsModel();
            var settingsView = new SettingsWindow();
            var settingsController = new SettingsController(settingsView, settingsModel, keyboardHook, mainWindow);

            var controller = new MainSupervisor(mainModel, mainWindow, keyboardHook, settingsController);
            controller.Show();
        }

        public static Version GetAppVersion()
        {
            return Assembly.GetEntryAssembly().GetName().Version;
        }

        private void OnDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            GlobalExceptionHandler(e.ExceptionObject as Exception);
            if (e.IsTerminating)
            {
                Environment.Exit(1);
            }
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            GlobalExceptionHandler(e.Exception);
        }

        private void GlobalExceptionHandler(Exception ex)
        {
            MessageBox.Show("An unexpected error has occurred."
                + Environment.NewLine
                + ex?.Message
                + Environment.NewLine + (_log != null ? "See the log file for details." : ex?.StackTrace),
                "Unexpected error",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );

            if (_log != null)
            {
                _log.Error("Unexpected error.", ex);
            }
        }
    }
}
