using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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
        //extern static int encrypt_cfb(IntPtr ctx, long p_length, char p_iv[16], char* p_src, char* r_dst);

        [DllImport(LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        extern static int decrypt_cfb(IntPtr ctx, ulong p_length, byte[] p_iv, byte[] p_src, byte[] r_dst);
        //extern static int decrypt_cfb(IntPtr ctx, long p_length, char p_iv[16], char* p_src, char* r_dst);

        [DllImport(LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        extern static bool set_key(IntPtr ctx, byte[] p_key);

        IntPtr ctx;

        public void Dispose()
        {
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

        public bool decrypt_cfb(byte[] p_iv, byte[] p_src, byte[] r_dst)
        {
            if (p_iv.Length != 16)
                throw new ArgumentOutOfRangeException($"The IV must be 16 bytes long. {nameof(p_iv)}");
            if (p_src.Length - 16 > r_dst.Length)
                throw new ArgumentOutOfRangeException($"The length of the source and output arrays is too much different. The difference should not exceed 16 bytes.");

            return decrypt_cfb(ctx, (ulong)p_src.Length, p_iv, p_src, r_dst) == 0;
        }
    }
}
