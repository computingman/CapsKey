using System;

namespace CapsKey.Model
{
    public class MainModel : ModelBase
    {
        public delegate void CapsStateChangedEventHandler(object sender, CapsStateChangeEventArgs args);

        public event CapsStateChangedEventHandler CapsStateChanged;

        private bool _isCapsActive;
        public bool IsCapsActive
        {
            get { return _isCapsActive; }
            set { SetCapsState(value, CapsStateSource.GUI); }
        }

        public string ToggleSwitchTooltip
        {
            get
            {
                return $"Caps Lock {(_isCapsActive ? "ON" : "off")}{Environment.NewLine}Press to toggle";
            }
        }

        public void SetCapsState(bool value, CapsStateSource source)
        {
            if (_isCapsActive != value)
            {
                _isCapsActive = value;
                RaisePropertyChanged(nameof(IsCapsActive));
                RaisePropertyChanged(nameof(ToggleSwitchTooltip));
                CapsStateChanged?.Invoke(this, new CapsStateChangeEventArgs(value, source));
            }
        }

        private RelayCommand _helpPressed;
        public RelayCommand HelpPressed
        {
            get { return _helpPressed; }
            set
            {
                _helpPressed = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand _minimisePressed;
        public RelayCommand MinimisePressed
        {
            get { return _minimisePressed; }
            set
            {
                _minimisePressed = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand _closePressed;
        public RelayCommand ClosePressed
        {
            get { return _closePressed; }
            set
            {
                _closePressed = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand _settingsPressed;
        public RelayCommand SettingsPressed
        {
            get { return _settingsPressed; }
            set
            {
                _settingsPressed = value;
                RaisePropertyChanged();
            }
        }
    }

    public class CapsStateChangeEventArgs : EventArgs
    {
        public bool IsCapsActive { get; private set; }

        public CapsStateSource Source { get; private set; }

        public CapsStateChangeEventArgs(bool isCapsActive, CapsStateSource source)
        {
            IsCapsActive = isCapsActive;
            Source = source;
        }
    }

    public enum CapsStateSource
    {
        GUI,
        CapsKey,
        ShortcutKey
    }
}
