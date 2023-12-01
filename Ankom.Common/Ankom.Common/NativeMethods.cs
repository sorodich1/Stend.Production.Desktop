using System;
using System.Runtime.InteropServices;

namespace Ankom.Common
{
    // Этот класс просто оболочка объектов Win32, который мы будем использовать
    // Обработка сообщения WM_SHOWME должна быть определена в WinProc главного окна приложения
    public static class NativeMethods
    {
        public const int HWND_BROADCAST = 0xffff;
        public const int SW_RESTORE = 9;
        public const int SW_HIDE = 0;
        public const int SW_SHOW = 5;
        public static readonly int WM_SHOWMODULE = RegisterWindowMessage("WM_SHOWMODULE");
        [DllImport("user32")]
        public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);
        [DllImport("user32")]
        public static extern bool SendMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);
        [DllImport("user32")]
        public static extern int RegisterWindowMessage(string message);

        // Sets the window to be foreground
        [DllImport("user32")]
        public static extern int SetForegroundWindow(IntPtr hwnd);

        // Activate or minimize a window
        [DllImport("user32")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetConsoleWindow();


    }
}
