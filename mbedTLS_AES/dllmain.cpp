
#include "mbedtls/aes.h"

#include <memory>

#ifdef _WIN32
#ifdef LIBRARY_EXPORTS
#    define LIBRARY_API __declspec(dllexport) 
#else
#    define LIBRARY_API __declspec(dllimport)
#endif
#else
#    define LIBRARY_API
#endif

extern "C" {
	LIBRARY_API void* create_context() {
		auto ctx = malloc(sizeof(mbedtls_aes_context));
		mbedtls_aes_init((mbedtls_aes_context*)ctx);
		return ctx;
	}

	LIBRARY_API void destroy_context(void* ctx) {
		mbedtls_aes_free((mbedtls_aes_context*)ctx);
		free((mbedtls_aes_context*)ctx);
	}

	LIBRARY_API bool set_key(void* ctx, const uint8_t* p_key) {
		int ret = mbedtls_aes_setkey_enc((mbedtls_aes_context*)ctx, p_key, 256);
		return ret ? 0 : 1;
	}

	LIBRARY_API int encrypt_cfb(void* ctx, uint64_t p_length, uint8_t* p_iv, const uint8_t* p_src, uint8_t* r_dst) {
		size_t iv_off = 0; // Ignore and assume 16-byte alignment.
		int ret = mbedtls_aes_crypt_cfb128((mbedtls_aes_context*)ctx, MBEDTLS_AES_ENCRYPT, (size_t)p_length, &iv_off, p_iv, p_src, r_dst);
		return ret ? 0 : 1;
	}

	LIBRARY_API int decrypt_cfb(void* ctx, uint64_t p_length, uint8_t* p_iv, const uint8_t* p_src, uint8_t* r_dst) {
		size_t iv_off = 0; // Ignore and assume 16-byte alignment.
		int ret = mbedtls_aes_crypt_cfb128((mbedtls_aes_context*)ctx, MBEDTLS_AES_DECRYPT, (size_t)p_length, &iv_off, p_iv, p_src, r_dst);
		return ret ? 0 : 1;
	}
}
