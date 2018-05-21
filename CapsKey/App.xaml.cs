using CapsKey.Helpers;
using CapsKey.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CapsKey
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var keyboardHook = new GlobalKeyboardHook();
            var model = new MainModel();
            var view = new MainWindow();
            var controller = new MainSupervisor(model, view, keyboardHook);
            controller.Show();
        }
    }
}
