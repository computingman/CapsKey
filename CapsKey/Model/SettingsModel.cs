using CapsKey.Properties;
using System.Windows.Forms;

namespace CapsKey.Model
{
    public class SettingsModel : ModelBase
    {
        public Settings Config { get { return Settings.Default; } }

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
    }
}
