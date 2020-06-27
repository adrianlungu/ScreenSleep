using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace ScreenSleep
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public const int HotkeyId = 4269;

        private static WindowInteropHelper _helper;

        // Modifier keys codes: Alt = 1, Ctrl = 2, Shift = 4, Win = 8
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public Settings()
        {
            InitializeComponent();

            _helper = ((MainWindow) Application.Current.MainWindow).Helper;

            ShortcutTextBox.Text = Properties.Settings.Default.SleepShortcut;
            TimerTextBox.Text = Properties.Settings.Default.SleepTimer.ToString();
            RunCheckbox.IsChecked = Properties.Settings.Default.RunOnStartup;
        }

        public static void SetupStartup()
        {
            try
            {
                var key =
                    Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                        "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                var curAssembly = Assembly.GetExecutingAssembly();

                if (Properties.Settings.Default.RunOnStartup)
                {
                    key.SetValue(curAssembly.GetName().Name, curAssembly.Location);
                }
                else
                {
                    key.DeleteValue(curAssembly.GetName().Name, false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error setting up application startup. " + ex.ToString(),
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        public static void SetupSleepShortcut(WindowInteropHelper helper)
        {
            var shortcut = Properties.Settings.Default.SleepShortcut.ToLower();

            string keyString = shortcut.Substring(shortcut.Length - 1);
            var modifiers = 0;

            if (shortcut.Contains("alt"))
            {
                modifiers += 1;
            }

            if (shortcut.Contains("ctrl"))
            {
                modifiers += 2;
            }

            if (shortcut.Contains("shift"))
            {
                modifiers += 4;
            }

            var keyConverter = new KeyConverter();
            var key = (Key) keyConverter.ConvertFromString(keyString);

            RegisterHotKey(helper.Handle, HotkeyId, modifiers, KeyInterop.VirtualKeyFromKey(key));
        }

        private void ShortcutTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            UnregisterHotKey(_helper.Handle, HotkeyId);

            // The text box grabs all input.
            e.Handled = true;

            // Fetch the actual shortcut key.
            Key key = (e.Key == Key.System ? e.SystemKey : e.Key);

            // Ignore modifier keys.
            if (key == Key.LeftShift || key == Key.RightShift
                                     || key == Key.LeftCtrl || key == Key.RightCtrl
                                     || key == Key.LeftAlt || key == Key.RightAlt
                                     || key == Key.LWin || key == Key.RWin)
            {
                return;
            }

            // Build the shortcut key name.
            StringBuilder shortcutText = new StringBuilder();
            if ((Keyboard.Modifiers & ModifierKeys.Control) != 0)
            {
                shortcutText.Append("Ctrl+");
            }
            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
            {
                shortcutText.Append("Shift+");
            }
            if ((Keyboard.Modifiers & ModifierKeys.Alt) != 0)
            {
                shortcutText.Append("Alt+");
            }
            shortcutText.Append(key.ToString());

            // Update the text box.
            ShortcutTextBox.Text = shortcutText.ToString();

            Properties.Settings.Default.SleepShortcut = shortcutText.ToString();
            Properties.Settings.Default.Save();

            SetupSleepShortcut(_helper);
        }

        private void RunCheckbox_OnChecked(object sender, RoutedEventArgs e)
        {
            if (RunCheckbox.IsChecked != null) Properties.Settings.Default.RunOnStartup = RunCheckbox.IsChecked.Value;

            Properties.Settings.Default.Save();
            SetupStartup();
        }

        private void TimerTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            int timer;

            var isNumeric = int.TryParse(e.Text, out timer);

            if (!isNumeric)
            {
                Properties.Settings.Default.SleepTimer = 1;
            }
            else
            {
                Properties.Settings.Default.SleepTimer = timer;
            }

            Properties.Settings.Default.Save();

            e.Handled = !isNumeric;
        }
    }
}
