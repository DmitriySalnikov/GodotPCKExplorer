using System;
using System.Runtime.InteropServices;

namespace GodotPCKExplorer
{
    public class mbedTLS : IDisposable
    {
        const string LIB_NAME = "mbedTLS_AES.dll";

        [DllImport(LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        extern static IntPtr create_context();

        [DllImport(LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        extern static void destroy_context(IntPtr ctx);

        [DllImport(LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        extern static int encrypt_cfb(IntPtr ctx, ulong p_length, byte[] p_iv, byte[] p_src, byte[] r_dst);
        //extern static int encrypt_cfb(IntPtr ctx, long p_length, char* p_iv, char* p_src, char* r_dst);

        [DllImport(LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        extern static int decrypt_cfb(IntPtr ctx, ulong p_length, byte[] p_iv, byte[] p_src, byte[] r_dst);
        //extern static int decrypt_cfb(IntPtr ctx, long p_length, char* p_iv, char* p_src, char* r_dst);

        [DllImport(LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        extern static bool set_key(IntPtr ctx, byte[] p_key);

        IntPtr ctx;

        public void Dispose()
        {
            if (ctx != IntPtr.Zero)
                destroy_context(ctx);
            ctx = IntPtr.Zero;
        }

        public mbedTLS()
        {
            ctx = create_context();
        }

        public bool set_key(byte[] key)
        {
            return set_key(ctx, key);
        }

        public bool decrypt_cfb(byte[] p_iv, byte[] p_src, int dst_size, out byte[] r_dst)
        {
            if (p_iv == null)
                throw new ArgumentNullException(nameof(p_iv));
            if (p_src == null)
                throw new ArgumentNullException(nameof(p_src));
            if (p_iv.Length != 16)
                throw new ArgumentOutOfRangeException($"The IV must be 16 bytes long. {nameof(p_iv)}");
            if (p_src.Length - 16 > dst_size)
                throw new ArgumentOutOfRangeException($"The length of the source and output arrays is too much different. The difference should not exceed 16 bytes.");

            var output = new byte[p_src.Length];
            var res = decrypt_cfb(ctx, (ulong)p_src.Length, p_iv, p_src, output) == 0;

            r_dst = new byte[dst_size];
            Array.Copy(output, r_dst, dst_size);

            return res;
        }
    }
}
