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

        public void SetCapsState(bool value, CapsStateSource source)
        {
            if (_isCapsActive != value)
            {
                _isCapsActive = value;
                RaisePropertyChanged(nameof(IsCapsActive));
                CapsStateChanged?.Invoke(this, new CapsStateChangeEventArgs(value, source));
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
