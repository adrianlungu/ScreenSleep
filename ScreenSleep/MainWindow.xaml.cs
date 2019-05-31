using System;
using System.Windows;
using System.Windows.Interop;

namespace ScreenSleep
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public WindowInteropHelper Helper;

        public MainWindow()
        {
            InitializeComponent();
        }
        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            Settings.SetupStartup();

            Helper = new WindowInteropHelper(this);

            var source = PresentationSource.FromVisual(this) as HwndSource;
            source.AddHook(WndProc);

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
            Screen.SetScreenState(ScreenState.Off);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == 0x0312 && wParam.ToInt32() == Settings.HotkeyId)
            {
                TurnOff_OnClick(null, null);
            }

            return IntPtr.Zero;
        }

    }
}
