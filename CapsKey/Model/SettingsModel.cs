using CapsKey.Properties;
using System.Collections.ObjectModel;
using System.Windows.Forms;

namespace CapsKey.Model
{
    public class SettingsModel : ModelBase
    {
        public Settings Config { get { return Settings.Default; } }

        public bool UseShortcutKey
        {
            get { return Config.UseShortcutKey; }
            set
            {
                if (Config.UseShortcutKey != value)
                {
                    Config.UseShortcutKey = value;
                    Config.Save();
                    RaisePropertyChanged();
                }
            }
        }

        public Keys GlobalShortcutKey
        {
            get { return Config.GlobalShortcutKey; }
            set
            {
                if (Config.GlobalShortcutKey != value)
                {
                    Config.GlobalShortcutKey = value;
                    Config.Save();
                    RaisePropertyChanged();
                }
            }
        }

        private Keys[] _allKeys;
        public Keys[] AllKeys
        {
            get { return _allKeys; }
            set
            {
                _allKeys = value;
                RaisePropertyChanged();
            }
        }

        private Keys _selectedKey;
        public Keys SelectedKey
        {
            get { return _selectedKey; }
            set
            {
                if (_selectedKey != value)
                {
                    _selectedKey = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool StartWithWindows
        {
            get { return Config.StartWithWindows; }
            set
            {
                if (Config.StartWithWindows != value)
                {
                    Config.StartWithWindows = value;
                    Config.Save();
                    RaisePropertyChanged();
                }
            }
        }

        public bool AlwaysOnTop
        {
            get { return Config.AlwaysOnTop; }
            set
            {
                if (Config.AlwaysOnTop != value)
                {
                    Config.AlwaysOnTop = value;
                    Config.Save();
                    RaisePropertyChanged();
                }
            }
        }

        public bool SuppressCapsKey
        {
            get { return Config.SuppressCapsKey; }
            set
            {
                if (Config.SuppressCapsKey != value)
                {
                    Config.SuppressCapsKey = value;
                    Config.Save();
                    RaisePropertyChanged();
                }
            }
        }

        private RelayCommand _viewLogPressed;
        public RelayCommand ViewLogPressed
        {
            get { return _viewLogPressed; }
            set
            {
                _viewLogPressed = value;
                RaisePropertyChanged();
            }
        }
    }
}
