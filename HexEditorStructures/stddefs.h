// Copyright (c) 2015-2022 by HHD Software Ltd.
// This file is part of the HHD Software Hex Editor Neo
// For usage and distribution policies, consult the license distributed with a product installation program

#pragma once
// Hex Editor Neo's Structure Viewer sample declaration file

// Constants and macros
#define MAX(a,b) ((a)>(b)?(a):(b))
#define MIN(a,b) ((a)<(b)?(a):(b))

#define TRUE 1
#define FALSE 0
#define NULL 0

// Type aliases

// Standard Windows types
typedef char CHAR;
typedef wchar_t WCHAR;
typedef short SHORT;
typedef int INT;
typedef long LONG;
typedef __int64 LONGLONG;

typedef unsigned char BYTE, UCHAR;
typedef unsigned short WORD, USHORT;
typedef unsigned int UINT;
typedef unsigned long DWORD, ULONG;
typedef unsigned __int64 ULONGLONG, FILETIME, QWORD;

// Sized integer types
typedef char int8, __int8, int8_t;
typedef short int16, __int16, int16_t;
typedef int int32, __int32, int32_t;
typedef __int64 int64, int64_t;

typedef unsigned char uint8, uint8_t;
typedef unsigned short uint16, uint16_t;
typedef unsigned int uint32, uint32_t;
typedef unsigned __int64 uint64, uint64_t;

[display(format("{{{0b16Xw8arf0}-{1b16Xw4arf0}-{2b16Xw4arf0}-{3b16Xw2arf0}{4b16Xw2arf0}-{5b16Xw2arf0}{6b16Xw2arf0}{7b16Xw2arf0}{8b16Xw2arf0}{9b16Xw2arf0}{10b16Xw2arf0}}",
	Data1,
	Data2,
	Data3,
	Data4[0],
	Data4[1],
	Data4[2],
	Data4[3],
	Data4[4],
	Data4[5],
	Data4[6],
	Data4[7]))]
struct GUID
{
	unsigned long  Data1;
	unsigned short Data2;
	unsigned short Data3;
	unsigned char  Data4[8];
};

// Expression type, as returned by built-in type() function
enum
{
	BooleanType,
	IntegerType,
	FloatingPointType,
	StringType,
	ReferenceType
};

typedef DWORD COLORREF;
typedef float	FLOAT;
typedef double	DOUBLE;
