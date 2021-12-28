using System.Runtime.InteropServices;

namespace ScreenSleep
{
    public enum ScreenState
    {
        On = -1,
        Off = 2,
        StandBy = 1
    }

    public static class Screen
    {
        [DllImport("user32.dll")]
        private static extern int SendMessage(int hWnd, int hMsg, int wParam, int lParam);

        // See https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-sendmessage
        private const int Broadcast = 0xFFFF;

        // See https://docs.microsoft.com/en-us/windows/win32/menurc/wm-syscommand
        // WM_SYSCOMMAND
        private const int WmSyscommand = 0x112;
        // SC_MONITORPOWER
        private const int ScMonitorPower = 0xF170;

        public static void SetScreenState(ScreenState state)
        {
            SendMessage(Broadcast, WmSyscommand, ScMonitorPower, (int)state);
        }

    }
}
