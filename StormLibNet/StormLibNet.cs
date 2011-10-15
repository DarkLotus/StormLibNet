using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace StormLibNet
{
    public static class StormLibNet
    {
        //File Searching
        [DllImport("StormLib.dll")]
        public static extern IntPtr SFileFindFirstFile( // This returns a handle to the MPQ Search Object
            IntPtr hMpq, // MPQ Handle
            [MarshalAs(UnmanagedType.LPStr)] string szMask,
            ref _SFILE_FIND_DATA lpFindFileData,
            [MarshalAs(UnmanagedType.LPStr)] string szListFile // Can be Null
            );
        [DllImport("StormLib.dll")]
        public static extern bool SFileFindNextFile(
            IntPtr hFind,
            ref _SFILE_FIND_DATA lpFindFileData
            );
        [DllImport("StormLib.dll")]
        public static extern bool SFileFindClose(
            IntPtr hFind 
            );
        

        //Reading files
        [DllImport("StormLib.dll")]
        public static extern bool SFileOpenFileEx(
            IntPtr hMpq,
             [MarshalAs(UnmanagedType.LPStr)] string szFileName,
             [MarshalAs(UnmanagedType.U4)] OpenFile dwSearchScope,
             out IntPtr phFile
            );

        [DllImport("StormLib.dll")]
        public static extern uint SFileGetFileSize( // return value is low 32 bits of file size
            IntPtr hFile, // File handle
            out uint pdwFileSizeHigh // High 32 bits of file size
            );
        [DllImport("StormLib.dll")]
        public static extern bool SFileReadFile(
            IntPtr hFile,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] lpBuffer,
            //IntPtr lpBuffer,
            [MarshalAs(UnmanagedType.I8)] Int64 dwToRead,
            out Int64 pdwRead
                        );

        [DllImport("StormLib.dll")]
        public static extern bool SFileCloseFile(
             IntPtr hFile
             );
       

        [DllImport("StormLib.dll")]
        public static extern bool SFileOpenArchive(
            [MarshalAs(UnmanagedType.LPStr)] string szMpqName,
            uint dwPriority,
            [MarshalAs(UnmanagedType.U4)] OpenArchiveFlags dwFlags,
            out IntPtr phMpq);
        [DllImport("StormLib.dll")]
        public static extern bool SFileCloseArchive(IntPtr hMpq);


        //bool   WINAPI SFileExtractFile(HANDLE hMpq, const char * szToExtract, const TCHAR * szExtracted, DWORD dwSearchScope = SFILE_OPEN_FROM_MPQ);
        [DllImport("StormLib.dll")]
        public static extern bool SFileExtractFile(
            IntPtr hMpq,
            [MarshalAs(UnmanagedType.LPStr)] string szToExtract,
            [MarshalAs(UnmanagedType.LPStr)] string szExtracted,
            [MarshalAs(UnmanagedType.U4)] OpenFile dwSearchScope);

        [DllImport("StormLib.dll")]
        public static extern bool SFileOpenPatchArchive(
            IntPtr hMpq,
            [MarshalAs(UnmanagedType.LPStr)] string szMpqName,
            [MarshalAs(UnmanagedType.LPStr)] string szPatchPathPrefix,
            uint dwFlags);





        //private List<MpqArchive> archives = new List<MpqArchive>();
        //private static IntPtr _mpqHandle = IntPtr.Zero;
        private static string _mpqDirectory = "";
        public static bool Init(string MpqDirectory)
        {
            _mpqDirectory = MpqDirectory;
            return true;
        }


    }

    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
    public struct _SFILE_FIND_DATA
    {

        /// char[260]
        [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 260)]
        public string cFileName;

        /// char*
        public System.IntPtr szPlainName;

        /// DWORD->unsigned int
        public uint dwHashIndex;

        /// DWORD->unsigned int
        public uint dwBlockIndex;

        /// DWORD->unsigned int
        public uint dwFileSize;

        /// DWORD->unsigned int
        public uint dwFileFlags;

        /// DWORD->unsigned int
        public uint dwCompSize;

        /// DWORD->unsigned int
        public uint dwFileTimeLo;

        /// DWORD->unsigned int
        public uint dwFileTimeHi;

        /// LCID->DWORD->unsigned int
        public uint lcLocale;
    }
    public enum OpenArchiveFlags : uint
    {
        NO_LISTFILE = 0x0010,   // Don't load the internal listfile
        NO_ATTRIBUTES = 0x0020,   // Don't open the attributes
        MFORCE_MPQ_V1 = 0x0040,   // Always open the archive as MPQ v 1.00, ignore the "wFormatVersion" variable in the header
        MCHECK_SECTOR_CRC = 0x0080,   // On files with MPQ_FILE_SECTOR_CRC, the CRC will be checked when reading file
        READ_ONLY = 0x0100,   // Open the archive for read-only access
        ENCRYPTED = 0x0200,   // Opens an encrypted MPQ archive (Example: Starcraft II installation)
    };

    // Values for SFileExtractFile
    public enum OpenFile : uint
    {
        FROM_MPQ = 0x00000000,   // Open the file from the MPQ archive
        PATCHED_FILE = 0x00000001,   // Open the file from the MPQ archive
        BY_INDEX = 0x00000002,   // The 'szFileName' parameter is actually the file index
        ANY_LOCALE = 0xFFFFFFFE,   // Reserved for StormLib internal use
        LOCAL_FILE = 0xFFFFFFFF,   // Open the file from the MPQ archive
    };
}
