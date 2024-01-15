using System;
using System.Runtime.InteropServices;

namespace NativeLibraryLoader
{
    internal static class Libdl
    {
        private static class Libdl1
        {
            private const string LibName = "libdl";

            [DllImport(LibName)]
            public static extern IntPtr dlopen(string fileName, int flags);

            [DllImport(LibName)]
            public static extern IntPtr dlsym(IntPtr handle, string name);

            [DllImport(LibName)]
            public static extern int dlclose(IntPtr handle);

            [DllImport(LibName)]
            public static extern string dlerror();
        }

        private static class Libdl2
        {
            private const string LibName = "libdl.so.2";

            [DllImport(LibName)]
            public static extern IntPtr dlopen(string fileName, int flags);

            [DllImport(LibName)]
            public static extern IntPtr dlsym(IntPtr handle, string name);

            [DllImport(LibName)]
            public static extern int dlclose(IntPtr handle);

            [DllImport(LibName)]
            public static extern string dlerror();
        }

        static Libdl()
        {
            try
            {
                Libdl1.dlerror();
                m_useLibdl1 = true;
            }
            catch
            {
            }
        }

        private static bool m_useLibdl1;

        public const int RTLD_NOW = 0x002;

        public static IntPtr dlopen(string fileName, int flags) => m_useLibdl1 ? Libdl1.dlopen(fileName, flags) : Libdl2.dlopen(fileName, flags);

        public static IntPtr dlsym(IntPtr handle, string name) => m_useLibdl1 ? Libdl1.dlsym(handle, name) : Libdl2.dlsym(handle, name);

        public static int dlclose(IntPtr handle) => m_useLibdl1 ? Libdl1.dlclose(handle) : Libdl2.dlclose(handle);

        public static string dlerror() => m_useLibdl1 ? Libdl1.dlerror() : Libdl2.dlerror();
    }
}
