using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using log4net;
using System.Threading.Tasks;
using CapsKey.Properties;

namespace CapsKey.Helpers
{
    public class KeyboardHelper
    {
        private static ILog _log = LogManager.GetLogger(typeof(KeyboardHelper));

        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        private static object _pressInProgressLock = new object();
        private static bool? _isCapsLocking;

        public static bool IsCapsKeyLocked()
        {
            return _isCapsLocking ?? IsCapsKeyLockedDirect();
        }

        private static bool IsCapsKeyLockedDirect()
        {
            return Control.IsKeyLocked(Keys.CapsLock);
        }

        public static void SetCapsLockState(bool desiredLockState, int retryCount = 0)
        {
            if (IsCapsKeyLockedDirect() != desiredLockState)
            {
                lock (_pressInProgressLock)
                {
                    if (IsCapsKeyLockedDirect() != desiredLockState)
                    {
                        _isCapsLocking = desiredLockState;

                        PressCapsLockKey();

                        Task.Delay(Settings.Default.CapsStateCheckDelay_ms);

                        if (IsCapsKeyLockedDirect() == desiredLockState)
                        {
                            _log.Debug("Caps set " + (desiredLockState ? "locked" : "unlocked"));
                            _isCapsLocking = null;
                            return;
                        }
                        else
                        {
                            _log.Warn($"Failed attempt {++retryCount} to set Caps {(desiredLockState ? "locked" : "unlocked")}");
                            if (retryCount <= Settings.Default.CapsStateRetryAttempts)
                            {
                                Task.Run(() => SetCapsLockState(desiredLockState, retryCount));
                            }
                            else
                            {
                                _isCapsLocking = null;  // Giving up after last failed retry.
                            }
                            return;
                        }
                    }
                }
            }
            else
            {
                _log.Debug("Caps is " + (desiredLockState ? "locked" : "unlocked"));

                if (retryCount > 0)
                {
                    _isCapsLocking = null;
                }
            }
        }

        private static void PressCapsLockKey()
        {
            const int KEYEVENTF_EXTENDEDKEY = 0x1;
            const int KEYEVENTF_KEYUP = 0x2;
            keybd_event(0x14, 0x45, KEYEVENTF_EXTENDEDKEY, (UIntPtr)0);
            keybd_event(0x14, 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, (UIntPtr)0);
        }
    }
}
