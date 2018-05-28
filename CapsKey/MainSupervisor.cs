using CapsKey.Model;
using CapsKey.Helpers;
using System.Windows.Forms;

namespace CapsKey
{
    public class MainSupervisor
    {
        private MainModel _model;
        private MainWindow _view;
        private GlobalKeyboardHook _keyboardHook;

        private SettingsController _settings;

        public MainSupervisor(MainModel model, MainWindow view, GlobalKeyboardHook keyboardHook, SettingsController settings)
        {
            _model = model;
            _view = view;

            _view.DataContext = model;

            _model.CapsStateChanged += OnModelCapsStateChanged;

            _keyboardHook = keyboardHook;
            _keyboardHook.KeyUp += OnKeyUp;

            _model.SetCapsState(KeyboardHelper.IsCapsKeyLocked(), CapsStateSource.CapsKey);

            _settings = settings;
            _model.SettingsPressed = new RelayCommand(OnSettingsPressed);
        }

        private void OnModelCapsStateChanged(object source, CapsStateChangeEventArgs e)
        {
            if (e.Source != CapsStateSource.CapsKey)
            {
                // The Caps Lock state has been toggled by the user. 
                // Keep the keyboard in-sync.
                KeyboardHelper.SetCapsLockState(e.IsCapsActive);
            }
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.CapsLock)
            {
                _model.SetCapsState(KeyboardHelper.IsCapsKeyLocked(), CapsStateSource.CapsKey);
            }
            else if (e.KeyCode == _settings.Model.GlobalShortcutKey)
            {
                _model.SetCapsState(!_model.IsCapsActive, CapsStateSource.ShortcutKey);
            }
        }

        private void OnSettingsPressed()
        {
            _settings.Show();
        }

        public void Show()
        {
            _view.Show();
        }
    }
}