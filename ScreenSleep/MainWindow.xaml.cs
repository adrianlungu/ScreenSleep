using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace ScreenSleep
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr hwnd, IntPtr hwndNewParent);

        // https://stackoverflow.com/questions/1399037/loading-a-wpf-window-without-showing-it
        private const int HWND_MESSAGE = -3;
        private IntPtr hwnd;
        private IntPtr oldParent;

        public WindowInteropHelper Helper;

        public MainWindow()
        {
            InitializeComponent();
        }
        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            Settings.SetupStartup();

            Helper = new WindowInteropHelper(this);
            Helper.EnsureHandle();

            if (PresentationSource.FromVisual(this) is HwndSource source)
            {
                source.AddHook(WndProc);

                hwnd = source.Handle;
                oldParent = SetParent(hwnd, (IntPtr) HWND_MESSAGE);
                Visibility = Visibility.Hidden;
                ShowActivated = false;
            }

            Settings.SetupSleepShortcut(Helper);
        }

        private void Exit_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Settings_OnClick(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new Settings();
            settingsWindow.Show();
        }

        private void TurnOff_OnClick(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                if (MyNotifyIcon.ContextMenu != null) MyNotifyIcon.ContextMenu.IsOpen = false;
            });

            Screen.SetScreenState(ScreenState.Off);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == 0x0312 && wParam.ToInt32() == Settings.HotkeyId)
            {
                Task.Delay(1000 * Properties.Settings.Default.SleepTimer).ContinueWith(t => TurnOff_OnClick(null, null));
            }

            return IntPtr.Zero;
        }

    }
}
