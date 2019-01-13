using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SAPAutomationJob
{
    public class ClipboardHelper
    {
        [DllImport("User32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsClipboardFormatAvailable(uint format);

        [DllImport("User32.dll", SetLastError = true)]
        private static extern IntPtr GetClipboardData(uint uFormat);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool SetClipboardData(uint uFormat, IntPtr data);

        [DllImport("User32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("User32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseClipboard();

        [DllImport("Kernel32.dll", SetLastError = true)]
        private static extern IntPtr GlobalLock(IntPtr hMem);

        [DllImport("Kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GlobalUnlock(IntPtr hMem);

        [DllImport("Kernel32.dll", SetLastError = true)]
        private static extern int GlobalSize(IntPtr hMem);

        private const uint CF_UNICODETEXT = 13U;
     


        /*
         * if you are usig SetText() and GetText() together 
         * make sure you Comment out CloseClipboard() and Marshal.FreeHGlobal(ptr) in SetText() method 
        */


        public static void SetText(string text)
        {
            try
            {
                if (!OpenClipboard(IntPtr.Zero)) return;

                var ptr = Marshal.StringToHGlobalUni(text);
                SetClipboardData(CF_UNICODETEXT, ptr);
                CloseClipboard();
                Marshal.FreeHGlobal(ptr);
            }
            finally
            {
                // CloseClipboard();
            }

        }

        public static string GetText()
        {
            if (!IsClipboardFormatAvailable(CF_UNICODETEXT))
                return null;

            try
            {
                if (!OpenClipboard(IntPtr.Zero))
                    return null;

                IntPtr handle = GetClipboardData(CF_UNICODETEXT);
                if (handle == IntPtr.Zero)
                    return null;

                IntPtr pointer = IntPtr.Zero;

                try
                {
                    pointer = GlobalLock(handle);
                    if (pointer == IntPtr.Zero)
                        return null;

                    int size = GlobalSize(handle);
                    byte[] buff = new byte[size];

                    Marshal.Copy(pointer, buff, 0, size);

                    return Encoding.Unicode.GetString(buff).TrimEnd('\0');
                }
                finally
                {
                    if (pointer != IntPtr.Zero)
                        GlobalUnlock(handle);
                }
            }
            finally
            {
                CloseClipboard();
            }
        }
    }
}

