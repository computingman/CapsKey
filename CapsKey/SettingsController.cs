using CapsKey.Model;
using CapsKey.Helpers;
using System.Windows.Forms;
using System;
using System.ComponentModel;
using log4net;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Reflection;
using System.IO;

namespace CapsKey
{
    public class SettingsController
    {
        private ILog _log = LogManager.GetLogger(typeof(SettingsController));

        private SettingsWindow _view;
        private GlobalKeyboardHook _keyboardHook;

        private MainWindow _mainWindow;

        private SettingsModel _model;

        public SettingsController(SettingsWindow view, SettingsModel model, GlobalKeyboardHook keyboardHook, MainWindow mainWindow)
        {
            _view = view;
            _view.DataContext = model;
            _model = model;

            _mainWindow = mainWindow;

            _keyboardHook = keyboardHook;
            HandleShortcutKeyChange();

            HandleAlwaysOnTopChange();

            PopulateAvailableShortcutKeys();
            
            _model.Config.PropertyChanged += OnSettingChanged;

            if (model.Config.IsFirstStart)
            {
                model.Config.StartWithWindows = RegistryHelper.StartWithWindows;
                model.Config.IsFirstStart = false;
            }

            _model.ViewLogPressed = new RelayCommand(OnViewLogPressed);
            _model.ClosePressed = new RelayCommand(OnClosePressed);
        }

        private void PopulateAvailableShortcutKeys()
        {
            var excludeKeys = new[]
            {
                Keys.Capital,
                Keys.CapsLock,
                Keys.ShiftKey,
                Keys.ControlKey
            };

            var allKeys = new List<Keys>();
            foreach (Keys key in Enum.GetValues(typeof(Keys)))
            {
                if (!excludeKeys.Contains(key) && ((int)key % (int)Keys.Modifiers) != 0)
                {
                    allKeys.Add(key);
                }
            }

            _model.AllKeys = allKeys.OrderBy(key => key.ToString()).ToArray();
        }

        private void HandleShortcutKeyChange()
        {
            if (_model.Config.UseShortcutKey)
            {
                _keyboardHook.HookKey = _model.Config.ShortcutKeySelected;
                _keyboardHook.HookWithAlt = _model.Config.ShortcutWithAltKey;
                _keyboardHook.HookWithShift = _model.Config.ShortcutWithShiftKey;
                _keyboardHook.HookWithControl = _model.Config.ShortcutWithControlKey;
            }
            else
            {
                _keyboardHook.HookKey = Keys.None;
            }
        }

        private void HandleAlwaysOnTopChange()
        {
            _mainWindow.Topmost = _model.Config.AlwaysOnTop;
        }

        private void OnSettingChanged(object sender, PropertyChangedEventArgs e)
        {
            _model.Config.Save();        
            _log.Info($"Updated setting {e.PropertyName} to: {_model.Config[e.PropertyName]}");

            if (e.PropertyName.Contains("Shortcut"))
            {
                HandleShortcutKeyChange();
            }
            else if (e.PropertyName == nameof(_model.Config.StartWithWindows))
            {
                RegistryHelper.StartWithWindows = _model.Config.StartWithWindows;
            }
            else if (e.PropertyName == nameof(_model.Config.AlwaysOnTop))
            {
                HandleAlwaysOnTopChange();
            }
            else if (e.PropertyName == nameof(_model.Config.SuppressCapsKey))
            {
                // Nothing to do here. Handled in `MainSupervisor::OnKeyUp()`.
            }
        }

        private void OnViewLogPressed()
        {
            var logFilePath = Assembly.GetEntryAssembly().Location.Replace(".exe", ".log");
            if (File.Exists(logFilePath))
            {
                Process.Start(logFilePath);
            }
            else
            {
                MessageBox.Show($@"Sorry, the file ""{logFilePath}"" was not found.", "Log file missing");
            }
        }

        private void OnClosePressed()
        {
            _view.Hide();
        }

        public void Show()
        {
            _view.Show();
            _view.Focus();
        }
    }
}
