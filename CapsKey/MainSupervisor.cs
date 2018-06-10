using CapsKey.Model;
using CapsKey.Helpers;
using System.Windows.Forms;
using log4net;
using System.Threading.Tasks;
using Application = System.Windows.Application;
using CapsKey.Properties;

namespace CapsKey
{
    public class MainSupervisor
    {
        private ILog _log = LogManager.GetLogger(typeof(MainSupervisor));

        private MainModel _model;
        private MainWindow _view;
        private GlobalKeyboardHook _keyboardHook;

        private SettingsController _settings;

        private HelpWindow _helpWindow;

        private bool _isSuppressingCapsKeyPress;
        private object _suppressingPressLock = new object();

        public MainSupervisor(MainModel model, MainWindow view, GlobalKeyboardHook keyboardHook, SettingsController settings, HelpWindow helpWindow)
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

            _helpWindow = helpWindow;

            _model.HelpPressed = new RelayCommand(OnHelpPressed);
            _model.MinimisePressed = new RelayCommand(OnMinimisePressed);
            _model.ClosePressed = new RelayCommand(OnClosePressed);
        }

        private void OnHelpPressed()
        {
            _helpWindow.Show();
        }

        private void OnMinimisePressed()
        {
            _view.Minimise();
        }

        private void OnClosePressed()
        {
            Application.Current.Shutdown();
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
                if (Settings.Default.SuppressCapsKey)
                {
                    Task.Run(() => SuppressCapsKeyPress());
                }
                else
                {
                    _model.SetCapsState(KeyboardHelper.IsCapsKeyLocked(), CapsStateSource.CapsKey);
                }
            }
            else
            {
                _model.SetCapsState(!_model.IsCapsActive, CapsStateSource.ShortcutKey);
            }
        }

        private void SuppressCapsKeyPress()
        {
            if (!_isSuppressingCapsKeyPress)
            {
                lock (_suppressingPressLock)
                {
                    if (!_isSuppressingCapsKeyPress)
                    {
                        _isSuppressingCapsKeyPress = true;
                        KeyboardHelper.SetCapsLockState(_model.IsCapsActive);
                        _isSuppressingCapsKeyPress = false;
                    }
                }
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