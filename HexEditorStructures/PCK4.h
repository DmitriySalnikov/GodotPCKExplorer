// HHD Software Hex Editor Neo
// Structure Definition File
//
// * Structure Viewer Overview: https://www.hhdsoftware.com/online-doc/hex/structure-viewer
// * Language Reference: https://www.hhdsoftware.com/online-doc/hex/language-reference

#include "stddefs.h"
#pragma script("get_doc_size.js")

function align(pos, align)
{
	if (pos % align)
	{
		return pos + (align - (pos % align));
	}
	return pos;
}

struct PackVersion
{
	int32 pck;
	
	[color_scheme("Timestamp")]
	int32 major;
	
	[color_scheme("VerMinor")]
	int32 minor;
	
	int32 revision;
};

struct FileData
{
	[color_scheme("AttributeHeader")]
	char data[file_size];
};

struct FileDataEncrypted
{
	[color_scheme("Signature")]
	char md5[16];
	
	[color_scheme("Size")]
	int64 enc_file_size;
	
	[color_scheme("Signature")]
	char iv[16];
	
	var ds = align(enc_file_size, 16);
	
	[color_scheme("AttributeHeader")]
	char data[ds];
};

struct FileIndex
{
	[color_scheme("Size")]
	int32 path_size;
	
	[color_scheme("HeaderNt")]
	char path[path_size];
	
	$shift_by(8);
	// skip for the file_offset to delay its init
	
	[color_scheme("Size")]
	int64 file_size;
	
	[color_scheme("Signature")]
	char md5[16];
	
	[color_scheme("Characteristics")]
	int32 flags;
	
	// flags - md5 - file_size - prev offset for this var
	$shift_by(- 4 - 16 - 8 - 8);
	if (flags & 1)
	{
		[color_scheme("Residency")]
		int64 file_offset as FileDataEncrypted  *(files_base);
	}
	else
	{
		[color_scheme("Residency")]
		int64 file_offset as FileData *(files_base);
	}
	
	// return to `flags`
	// file_size + md5 + flags
	$shift_by(8 + 16 + 4);
};

struct FileIndexEncrypted
{
	[color_scheme("Signature")]
	char md5[16];
	
	[color_scheme("Size")]
	int64 index_size;
	
	[color_scheme("Signature")]
	char iv[16];
	
	var ds = align(index_size, 16);
	
	[color_scheme("AttributeHeader")]
	char data[ds];
};

public struct GodotPCK4
{
	var start_of_pck = 0;
	var doc_file_size = GetDocumentSize();

	// Search for a regular or embedded PCK

hidden:
	int32 test_magic;
visible:
	if (test_magic == 0x43504447)
	{
		$shift_by(-4);
	}
	else
	{
		$shift_by(doc_file_size-4-4-8);
		
		[color_scheme("Size")]
		int64 pck_data_size;
		[color_scheme("FileRecordMagic")]
		int32 footer_magic;
		$assert(footer_magic == 0x43504447, "Invalid Footer Magic number");
		
		$shift_by(-8 - 4 - pck_data_size);
		//$remove_to(ref(test_magic));
	}
	
visible:
	[color_scheme("FileRecordMagic")]
	int32 magic;
	
	$assert(magic == 0x43504447, "Invalid Magic number");
	PackVersion version;
	
	[color_scheme("Characteristics")]
	int32 flags;
	
	[color_scheme("Size")]
	int64 files_base;
	
hidden:
	[color_scheme("AttributeHeader")]
	int32 reserved[16];

visible:
	[color_scheme("HeaderNt")]
	int32 file_count;
	
	if (!(flags & 1))
	{
		FileIndex files[file_count];
	}
	else
	{
		FileIndexEncrypted files_encrypted;
	}
};
