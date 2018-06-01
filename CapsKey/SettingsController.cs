using CapsKey.Model;
using CapsKey.Helpers;
using System.Windows.Forms;
using System;
using System.ComponentModel;
using log4net;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace CapsKey
{
    public class SettingsController
    {
        private ILog _log = LogManager.GetLogger(typeof(SettingsController));

        private SettingsWindow _view;
        private GlobalKeyboardHook _keyboardHook;

        private MainWindow _mainWindow;

        public SettingsModel Model { get; private set; }

        public SettingsController(SettingsWindow view, SettingsModel model, GlobalKeyboardHook keyboardHook, MainWindow mainWindow)
        {
            _view = view;
            _view.DataContext = model;
            Model = model;

            _mainWindow = mainWindow;

            Model.SelectedKey = Model.GlobalShortcutKey;
            _keyboardHook = keyboardHook;
            HandleShortcutKeyChange();
            _keyboardHook.HookedKeys.Add(Keys.CapsLock);

            HandleAlwaysOnTopChange();

            var allKeys = new List<Keys>();
            foreach (Keys key in Enum.GetValues(typeof(Keys)))
            {
                allKeys.Add(key);
            }
            Model.AllKeys = allKeys.OrderBy(key => key.ToString()).ToArray();

            Model.PropertyChanged += OnModelPropertyChanged;
            Model.Config.PropertyChanged += OnSettingChanged;

            Model.ViewLogPressed = new RelayCommand(OnViewLogPressed);
        }

        private void OnModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SettingsModel.UseShortcutKey)
                || e.PropertyName == nameof(SettingsModel.GlobalShortcutKey))
            {
                HandleShortcutKeyChange();
            }
            else if (e.PropertyName == nameof(SettingsModel.SelectedKey))
            {
                // ToDo: support modifier keys (e.g. Shift, Ctrl, etc)?
                Model.GlobalShortcutKey = Model.SelectedKey;
            }
            else if (e.PropertyName == nameof(SettingsModel.StartWithWindows))
            {
                RegistryHelper.StartWithWindows = Model.StartWithWindows;
            }
            else if (e.PropertyName == nameof(SettingsModel.AlwaysOnTop))
            {
                HandleAlwaysOnTopChange();
            }
            else if (e.PropertyName == nameof(SettingsModel.SuppressCapsKey))
            {
                // Nothing to do here. Handled in `MainSupervisor::OnKeyUp()`.
            }
        }

        private void HandleShortcutKeyChange()
        {
            if (Model.UseShortcutKey)
            {
                _keyboardHook.HookShortcutKey(Model.GlobalShortcutKey);
            }
            else
            {
                _keyboardHook.HookShortcutKey(Keys.None);
            }
        }

        private void HandleAlwaysOnTopChange()
        {
            _mainWindow.Topmost = Model.AlwaysOnTop;
        }

        private void OnSettingChanged(object sender, PropertyChangedEventArgs e)
        {
            _log.Info($"Updated setting {e.PropertyName} to: {Model.Config[e.PropertyName]}");
        }

        private void OnViewLogPressed()
        {
            Process.Start("CapsKey.log");
        }

        public void Show()
        {
            _view.Show();
            _view.Focus();
        }
    }
}
