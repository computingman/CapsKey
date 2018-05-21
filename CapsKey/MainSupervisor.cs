using System.ComponentModel;
using CapsKey.Model;
using CapsKey.Helpers;
using System.Windows.Forms;

namespace CapsKey
{
    internal class MainSupervisor
    {
        private MainModel _model;
        private MainWindow _view;
        private GlobalKeyboardHook _keyboardHook;

        public MainSupervisor(MainModel model, MainWindow view, GlobalKeyboardHook keyboardHook)
        {
            _model = model;
            _view = view;

            _view.DataContext = model;

            _model.PropertyChanged += OnModelPropertyChanged;

            _keyboardHook = keyboardHook;
            _keyboardHook.HookedKeys.Add(Keys.CapsLock);
            _keyboardHook.KeyUp += OnKeyUp;

            _model.IsCapsActive = KeyboardHelper.IsCapsKeyLocked();
        }

        private void OnModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_model.IsCapsActive))
            {
                // The Caps Lock state has been toggled. 
                // Keep the keyboard in-sync (if it's not already, i.e. if the user toggled the state via the GUI)...
                KeyboardHelper.SetCapsLockState(_model.IsCapsActive);
            }
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.CapsLock)
            {
                _model.IsCapsActive = KeyboardHelper.IsCapsKeyLocked();
            }
        }

        public void Show()
        {
            _view.Show();
        }
    }
}