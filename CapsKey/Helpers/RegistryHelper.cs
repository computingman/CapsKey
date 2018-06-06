using Microsoft.Win32;
using System.IO;
using System.Reflection;

namespace CapsKey.Helpers
{
    public static class RegistryHelper
    {
        private const string StartWithWindowsRoot = @"HKEY_CURRENT_USER\";
        private const string StartWithWindowsSubKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string AppValue = "CapsKey";

        public static bool StartWithWindows
        {
            get
            {
                return File.Exists(Registry.GetValue(StartWithWindowsRoot + StartWithWindowsSubKey, AppValue, null) as string);
            }
            set
            {
                if (value)
                {
                    var exePath = Assembly.GetEntryAssembly().Location;
                    Registry.SetValue(StartWithWindowsRoot + StartWithWindowsSubKey, AppValue, exePath);
                }
                else
                {
                    using (var key = Registry.CurrentUser.OpenSubKey(StartWithWindowsSubKey, true))
                    {
                        if (key.GetValue(AppValue, null) != null)
                        {
                            key.DeleteValue(AppValue);
                        }
                    }
                }
            }
        }
    }
}
