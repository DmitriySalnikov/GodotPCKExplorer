SOURCE = dllmain.c \
		mbedtls/library/aes.c \
		mbedtls/library/aesni.c \
		mbedtls/library/platform_util.c
CC = gcc
CFLAGS = -Wall -shared -fPIC -fvisibility=hidden -O3 -flto
LIBRARY = 
INCLUDE_PATH = -Imbedtls/include

PLATFORM ?= linux
ARCH ?= x64

ifeq ($(ARCH), x64)
	CFLAGS += -m64
else ifeq ($(ARCH), x86)
	CFLAGS += -m32 -mpclmul -msse2 -maes
else ifeq ($(ARCH), universal)
	CFLAGS += -arch x86_64 -arch arm64 
else
	$(error Unsupported architecture: $(ARCH))
endif

EXTENSION = 
ifeq ($(PLATFORM), linux)
	EXTENSION = .so
else ifeq ($(PLATFORM), mac)
	CFLAGS += -mmacosx-version-min=10.14
	EXTENSION = .dylib
else
	$(error Unsupported architecture: $(PLATFORM))
endif

TARGET = ../GodotPCKExplorer/mbedTLS/$(PLATFORM)_$(ARCH)/mbedTLS_AES$(EXTENSION)

all: create_directory $(TARGET)

create_directory:
	mkdir -p $(dir $(TARGET))

$(TARGET): $(SOURCE)
	$(CC) -xc $(CFLAGS) $(SOURCE) -o $(TARGET) $(LIB_PATH) $(LIBRARY) $(INCLUDE_PATH)

clean:
	rm -f $(TARGET)
