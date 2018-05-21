using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace CapsKey.Helpers
{
    public class KeyboardHelper
    {
        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        private static bool? _isCapsLocking;

        public static bool IsCapsKeyLocked()
        {
            return _isCapsLocking ?? Control.IsKeyLocked(Keys.CapsLock);
        }

        public static void SetCapsLockState(bool isLocked)
        {
            if (IsCapsKeyLocked() != isLocked)
            {
                _isCapsLocking = isLocked;
                PressCapsLockKey();
                _isCapsLocking = null;
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
