// HHD Software Hex Editor Neo
// Structure Definition File
//
// * Structure Viewer Overview: https://www.hhdsoftware.com/online-doc/hex/structure-viewer
// * Language Reference: https://www.hhdsoftware.com/online-doc/hex/language-reference

#include "stddefs.h"
#pragma script("get_doc_size.js")

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

struct FileHeader
{
	[color_scheme("Size")]
	int32 path_size;
	[color_scheme("HeaderNt")]
	char path[path_size];
	[color_scheme("Residency")]
	int64 file_offset as FileData *(files_base);
	[color_scheme("Size")]
	int64 file_size;
	[color_scheme("Signature")]
	char md5[16];
	[color_scheme("Characteristics")]
	int32 flags;
};

public struct GodotPCK4
{
	var start_of_pck = 0;
	var pck_file_size = GetDocumentSize();
	
	// TODO: add support for embeded files
	[color_scheme("FileRecordMagic")]
	int32 magic;
	$assert(magic == 0x43504447, "Invalid Magic number");
	PackVersion version;
	
	[color_scheme("Residency")]
	int32 flags;
	$assert(!(flags & 1), "The PCK is encrypted. Use other tools to inspect this file!");
	
	[color_scheme("Size")]
	int64 files_base;
hidden:
	int32 reserved[16];
visible:
	[color_scheme("Size")]
	int32 file_count;
	FileHeader files[file_count];
};
