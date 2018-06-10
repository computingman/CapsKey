using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace CapsKey.Helpers
{
    /// <summary>
    /// From https://www.codeproject.com/Articles/19004/A-Simple-C-Global-Low-Level-Keyboard-Hook
    /// A class that manages a global low level keyboard hook
    /// </summary>
    public class GlobalKeyboardHook
    {
		#region Constant, Structure and Delegate Definitions
		/// <summary>
		/// defines the callback type for the hook
		/// </summary>
		public delegate int KeyboardHookProc(int code, int wParam, ref KeyboardHookStruct lParam);

		public struct KeyboardHookStruct
        {
			public int vkCode;
			public int scanCode;
			public int flags;
			public int time;
			public int dwExtraInfo;
		}

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x100;
        private const int WM_KEYUP = 0x101;
        private const int WM_SYSKEYDOWN = 0x104;
        private const int WM_SYSKEYUP = 0x105;

        private const int VK_SHIFT = 0x10;
        private const int VK_CONTROL = 0x11;
        private const int VK_MENU = 0x12;
        private const int MODIFIER_MASK = 0x8000;
        #endregion

        #region Instance Variables
        /// <summary>
        /// A key to watch for (in addition to CapsLock, which is always hooked).
        /// </summary>
        public Keys HookKey { get; set; } = Keys.None;

        public bool HookWithAlt { get; set; }
        public bool HookWithShift { get; set; }
        public bool HookWithControl { get; set; }

        /// <summary>
        /// Handle to the hook, need this to unhook and call the next hook
        /// </summary>
        private IntPtr hhook = IntPtr.Zero;

        private KeyboardHookProc _callback;
		#endregion

		#region Events
		/// <summary>
		/// Occurs when the hooked keys are pressed and then released.
		/// </summary>
		public event KeyEventHandler KeyUp;
		#endregion

		#region Constructors and Destructors
		/// <summary>
		/// Initializes a new instance of the <see cref="globalKeyboardHook"/> class and installs the keyboard hook.
		/// </summary>
		public GlobalKeyboardHook()
        {
            _callback = new KeyboardHookProc(HookProc);
			Hook();
		}

		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="globalKeyboardHook"/> is reclaimed by garbage collection and uninstalls the keyboard hook.
		/// </summary>
		~GlobalKeyboardHook()
        {
			Unhook();
            _callback = null;
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Installs the global hook
		/// </summary>
		public void Hook()
        {
			IntPtr hInstance = LoadLibrary("User32");
			hhook = SetWindowsHookEx(WH_KEYBOARD_LL, _callback, hInstance, 0);
		}

        /// <summary>
        /// Uninstalls the global hook
        /// </summary>
        public void Unhook()
        {
			UnhookWindowsHookEx(hhook);
		}

		/// <summary>
		/// The callback for the keyboard hook
		/// </summary>
		/// <param name="code">The hook code, if it isn't >= 0, the function shouldn't do anyting</param>
		/// <param name="wParam">The event type</param>
		/// <param name="lParam">The keyhook event information</param>
		/// <returns></returns>
		public int HookProc(int code, int wParam, ref KeyboardHookStruct lParam)
        {
			if (code >= 0 && KeyUp != null && (wParam == WM_KEYUP || wParam == WM_SYSKEYUP))
            {
				var key = (Keys)lParam.vkCode;
                if (key == Keys.CapsLock
                    || (key == HookKey
                        && (!HookWithAlt || (GetKeyState(VK_MENU) & MODIFIER_MASK) != 0)
                        && (!HookWithShift || (GetKeyState(VK_SHIFT) & MODIFIER_MASK) != 0)
                        && (!HookWithControl || (GetKeyState(VK_CONTROL) & MODIFIER_MASK) != 0)))
                {
                    KeyEventArgs eventArgs = new KeyEventArgs(key);
                    KeyUp(this, eventArgs);
                    if (eventArgs.Handled)
                        return 1;
                }
			}
			return CallNextHookEx(hhook, code, wParam, ref lParam);
		}
		#endregion

		#region DLL imports
		/// <summary>
		/// Sets the windows hook, do the desired event, one of hInstance or threadId must be non-null
		/// </summary>
		/// <param name="idHook">The id of the event you want to hook</param>
		/// <param name="callback">The callback.</param>
		/// <param name="hInstance">The handle you want to attach the event to, can be null</param>
		/// <param name="threadId">The thread you want to attach the event to, can be null</param>
		/// <returns>a handle to the desired hook</returns>
		[DllImport("user32.dll")]
		static extern IntPtr SetWindowsHookEx(int idHook, KeyboardHookProc callback, IntPtr hInstance, uint threadId);

		/// <summary>
		/// Unhooks the windows hook.
		/// </summary>
		/// <param name="hInstance">The hook handle that was returned from SetWindowsHookEx</param>
		/// <returns>True if successful, false otherwise</returns>
		[DllImport("user32.dll")]
		static extern bool UnhookWindowsHookEx(IntPtr hInstance);

		/// <summary>
		/// Calls the next hook.
		/// </summary>
		/// <param name="idHook">The hook id</param>
		/// <param name="nCode">The hook code</param>
		/// <param name="wParam">The wparam.</param>
		/// <param name="lParam">The lparam.</param>
		/// <returns></returns>
		[DllImport("user32.dll")]
		static extern int CallNextHookEx(IntPtr idHook, int nCode, int wParam, ref KeyboardHookStruct lParam);

		/// <summary>
		/// Loads the library.
		/// </summary>
		/// <param name="lpFileName">Name of the library</param>
		/// <returns>A handle to the library</returns>
		[DllImport("kernel32.dll")]
		static extern IntPtr LoadLibrary(string lpFileName);
        
        /// <summary>
        /// Gets the state of modifier keys for a given keycode.
        /// </summary>
        /// <param name="keyCode">The keyCode</param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        public static extern short GetKeyState(int keyCode);
        #endregion
    }
}
