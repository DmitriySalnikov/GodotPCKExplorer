using System;
using System.Runtime.InteropServices;

namespace GodotPCKExplorer
{
    public class mbedTLS : IDisposable
    {
        const string LIB_NAME = "mbedTLS_AES.dll";
        public const int CHUNK_SIZE = 16;

        [DllImport(LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr create_context();

        [DllImport(LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        static extern void destroy_context(IntPtr ctx);

        [DllImport(LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        static extern unsafe bool encrypt_cfb(IntPtr ctx, ulong srcLength, byte* p_iv, byte* p_src, byte* r_dst);
        //extern static int encrypt_cfb(IntPtr ctx, long p_src_length, char* p_iv, char* p_src, char* r_dst);

        [DllImport(LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        static extern unsafe bool decrypt_cfb(IntPtr ctx, ulong srcLength, byte* p_iv, byte* p_src, byte* r_dst);
        //extern static int decrypt_cfb(IntPtr ctx, long p_src_length, char* p_iv, char* p_src, char* r_dst);

        [DllImport(LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        static extern bool set_key(IntPtr ctx, byte[] p_key);

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
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (key.Length != 32)
                throw new ArgumentOutOfRangeException(nameof(key));

            return set_key(ctx, key);
        }

        public unsafe bool decrypt_cfb(Memory<byte> p_iv, ReadOnlyMemory<byte> p_src, Memory<byte> r_dst)
        {
            if (p_iv.Length != CHUNK_SIZE)
                throw new ArgumentOutOfRangeException($"The IV must be {CHUNK_SIZE} bytes long. {nameof(p_iv.Length)}");
            if (p_src.Length % CHUNK_SIZE != 0)
                throw new ArgumentOutOfRangeException($"The input data must be a multiple of {CHUNK_SIZE}. {nameof(p_src.Length)}");

            fixed (byte* p_iv_ptr = &MemoryMarshal.GetReference(p_iv.Span))
            fixed (byte* p_src_ptr = &MemoryMarshal.GetReference(p_src.Span))
            fixed (byte* r_dst_ptr = &MemoryMarshal.GetReference(r_dst.Span))
                return decrypt_cfb(ctx, (ulong)p_src.Length, p_iv_ptr, p_src_ptr, r_dst_ptr);
        }

        public unsafe bool encrypt_cfb(Memory<byte> p_iv, ReadOnlyMemory<byte> p_src, Memory<byte> r_dst, out long r_dst_size)
        {
            if (p_iv.Length != CHUNK_SIZE)
                throw new ArgumentOutOfRangeException($"The IV must be {CHUNK_SIZE} bytes long. {nameof(p_iv.Length)}");

            r_dst_size = PCKUtils.AlignAddress(p_src.Length, CHUNK_SIZE);

            fixed (byte* p_iv_ptr = &MemoryMarshal.GetReference(p_iv.Span))
            fixed (byte* p_src_ptr = &MemoryMarshal.GetReference(p_src.Span))
            fixed (byte* r_dst_ptr = &MemoryMarshal.GetReference(r_dst.Span))
                return encrypt_cfb(ctx, (ulong)r_dst_size, p_iv_ptr, p_src_ptr, r_dst_ptr);
        }
    }
}
