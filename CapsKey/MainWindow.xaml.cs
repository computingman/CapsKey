﻿using CapsKey.Properties;
using System;
using System.Windows;
using System.Windows.Input;
using Point = System.Drawing.Point;

namespace CapsKey
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            SetStartupLocation();
        }

        private void SetStartupLocation()
        {
            WindowStartupLocation = WindowStartupLocation.Manual;

            Point location = Settings.Default.MainWindowLocation;

            Left = location.X;
            Top = location.Y;

            if (!IsWindowLocationOnScreen())
            {
                // The previously-saved location is now off-screen (e.g. the number of monitors or the monitor resolution was reduced).
                Left = 0;
                Top = 0;
            }
        }

        private void OnWindowLocationChanged()
        {
            if (!IsWindowLocationOnScreen())
                return;

            // ToDo: only save & log last location when dragging...
            var newLocation = new Point((int)Left, (int)Top);
            if (Settings.Default.MainWindowLocation != newLocation)
            {
                Settings.Default.MainWindowLocation = newLocation;
                Settings.Default.Save();
            }
        }

        public void Minimise()
        {
            WindowState = WindowState.Minimized;
        }

        /// <returns>True if the window is positioned in a visible location,
        /// or False if the window is off-screen (e.g. due to being minimised or at a location on a detached monitor).</returns>
        private bool IsWindowLocationOnScreen()
        {
            return (Left >= SystemParameters.VirtualScreenLeft
                && Top >= SystemParameters.VirtualScreenTop
                && Left < SystemParameters.VirtualScreenLeft + SystemParameters.VirtualScreenWidth
                && Top < SystemParameters.VirtualScreenTop + SystemParameters.VirtualScreenHeight);
        }

        private void OnWindowMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
            OnWindowLocationChanged();
        }
    }
}
