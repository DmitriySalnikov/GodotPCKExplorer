using System;
using System.Runtime.InteropServices;

namespace NativeLibraryLoader
{
    internal static class Kernel32
    {
        [DllImport("kernel32")]
        public static extern IntPtr LoadLibrary(string fileName);

        [DllImport("kernel32")]
        public static extern IntPtr GetProcAddress(IntPtr module, string procName);

        [DllImport("kernel32")]
        public static extern int FreeLibrary(IntPtr module);
    }
}
