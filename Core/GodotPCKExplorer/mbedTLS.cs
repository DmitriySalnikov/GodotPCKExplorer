using NativeLibraryLoader;
using System;
using System.Runtime.InteropServices;

namespace GodotPCKExplorer
{
    public sealed class mbedTLS : IDisposable
    {
        const string LIB_NAME = "mbedTLS_AES";
        public const int CHUNK_SIZE = 16;

        delegate IntPtr create_context_dg();
        delegate void destroy_context_dg(IntPtr ctx);
        unsafe delegate int encrypt_cfb_dg(IntPtr ctx, ulong srcLength, byte* p_iv, byte* p_src, byte* r_dst);
        unsafe delegate int decrypt_cfb_dg(IntPtr ctx, ulong srcLength, byte* p_iv, byte* p_src, byte* r_dst);
        delegate int set_key_dg(IntPtr ctx, byte[] p_key);

        static create_context_dg? n_create_context;
        static destroy_context_dg? n_destroy_context;
        static encrypt_cfb_dg? n_encrypt_cfb;
        static decrypt_cfb_dg? n_decrypt_cfb;
        static set_key_dg? n_set_key;

        IntPtr ctx;

        public void Dispose()
        {
            if (ctx != IntPtr.Zero)
                n_destroy_context?.Invoke(ctx);
            ctx = IntPtr.Zero;
        }

        static T load_method<T>(NativeLibrary lib) where T : class
        {
            var name = typeof(T).Name.Replace("_dg", "");
            return lib.LoadFunction<T>(name);
        }

        internal static void LoadMethods(NativeLibrary lib)
        {
            n_create_context ??= load_method<create_context_dg>(lib);
            n_destroy_context ??= load_method<destroy_context_dg>(lib);
            n_encrypt_cfb ??= load_method<encrypt_cfb_dg>(lib);
            n_decrypt_cfb ??= load_method<decrypt_cfb_dg>(lib);
            n_set_key ??= load_method<set_key_dg>(lib);
        }

        internal static void UnloadMethods()
        {
            n_create_context = null;
            n_destroy_context = null;
            n_encrypt_cfb = null;
            n_decrypt_cfb = null;
            n_set_key = null;
        }

        public mbedTLS()
        {
            ctx = n_create_context?.Invoke() ?? throw new NullReferenceException(nameof(n_create_context));
        }

        public bool set_key(byte[] key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (key.Length != 32)
                throw new ArgumentOutOfRangeException(nameof(key));
            if (n_set_key == null)
                throw new ArgumentOutOfRangeException(nameof(n_set_key));

            return n_set_key(ctx, key) == 0;
        }

        public unsafe bool decrypt_cfb(Memory<byte> p_iv, ReadOnlyMemory<byte> p_src, Memory<byte> r_dst)
        {
            if (p_iv.Length != CHUNK_SIZE)
                throw new ArgumentOutOfRangeException($"The IV must be {CHUNK_SIZE} bytes long. {nameof(p_iv.Length)}");
            if (p_src.Length % CHUNK_SIZE != 0)
                throw new ArgumentOutOfRangeException($"The input data must be a multiple of {CHUNK_SIZE}. {nameof(p_src.Length)}");
            if (n_decrypt_cfb == null)
                throw new ArgumentOutOfRangeException(nameof(n_decrypt_cfb));

            fixed (byte* p_iv_ptr = &MemoryMarshal.GetReference(p_iv.Span))
            fixed (byte* p_src_ptr = &MemoryMarshal.GetReference(p_src.Span))
            fixed (byte* r_dst_ptr = &MemoryMarshal.GetReference(r_dst.Span))
                return n_decrypt_cfb(ctx, (ulong)p_src.Length, p_iv_ptr, p_src_ptr, r_dst_ptr) == 0;
        }

        public unsafe bool encrypt_cfb(Memory<byte> p_iv, ReadOnlyMemory<byte> p_src, Memory<byte> r_dst, out long r_dst_size)
        {
            if (p_iv.Length != CHUNK_SIZE)
                throw new ArgumentOutOfRangeException($"The IV must be {CHUNK_SIZE} bytes long. {nameof(p_iv.Length)}");
            if (n_encrypt_cfb == null)
                throw new ArgumentOutOfRangeException(nameof(n_encrypt_cfb));

            r_dst_size = PCKUtils.AlignAddress(p_src.Length, CHUNK_SIZE);

            fixed (byte* p_iv_ptr = &MemoryMarshal.GetReference(p_iv.Span))
            fixed (byte* p_src_ptr = &MemoryMarshal.GetReference(p_src.Span))
            fixed (byte* r_dst_ptr = &MemoryMarshal.GetReference(r_dst.Span))
                return n_encrypt_cfb(ctx, (ulong)r_dst_size, p_iv_ptr, p_src_ptr, r_dst_ptr) == 0;
        }
    }
}
