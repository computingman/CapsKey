using CapsKey.Model;
using CapsKey.Helpers;
using System.Windows.Forms;
using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using log4net;
using CapsKey.Properties;
using System.Collections.Generic;
using System.Linq;

namespace CapsKey
{
    public class SettingsController
    {
        private ILog _log = LogManager.GetLogger(typeof(SettingsController));

        private SettingsWindow _view;
        private GlobalKeyboardHook _keyboardHook;

        public SettingsModel Model { get; private set; }

        public SettingsController(SettingsWindow view, SettingsModel model, GlobalKeyboardHook keyboardHook)
        {
            _view = view;
            _view.DataContext = model;
            Model = model;

            _keyboardHook = keyboardHook;
            HandleShortcutKeyChange();
            _keyboardHook.HookedKeys.Add(Keys.CapsLock);

            var allKeys = new List<Keys>();
            foreach (Keys key in Enum.GetValues(typeof(Keys)))
            {
                allKeys.Add(key);
            }
            Model.AllKeys = allKeys.OrderBy(key => key.ToString()).ToArray();

            Model.PropertyChanged += OnModelPropertyChanged;
            Model.Config.PropertyChanged += OnSettingChanged;
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
            // ToDo: support "Always on top".
            // ToDo: support "Suppress CAPS key".
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

        private void OnSettingChanged(object sender, PropertyChangedEventArgs e)
        {
            _log.Info($"Updated setting {e.PropertyName} to: {Model.Config[e.PropertyName]}");
        }

        public void Show()
        {
            _view.Show();
        }
    }
}
