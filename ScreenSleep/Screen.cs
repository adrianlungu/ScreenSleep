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

        public static void SetScreenState(ScreenState state)
        {
            SendMessage(0xFFFF, 0x112, 0xF170, (int)state);
        }

    }
}
