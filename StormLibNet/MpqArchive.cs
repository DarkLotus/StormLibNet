using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Win32;

namespace StormLibNet
{
    public class MpqArchive : IDisposable
    {
        private IntPtr handle = IntPtr.Zero;

        public MpqArchive(string file, uint Prio, OpenArchiveFlags Flags)
        {
            bool r = Open(file, Prio, Flags);
        }

        public bool IsOpen { get { return handle != IntPtr.Zero; } }

        private bool Open(string file, uint Prio, OpenArchiveFlags Flags)
        {
            bool r = StormLibNet.SFileOpenArchive(file, Prio, Flags, out handle);
            //if (r)
            //OpenPatch(file);
            return r;
        }

     
        public void Dispose()
        {
            Close();
        }

        public bool Close()
        {
            bool r = StormLibNet.SFileCloseArchive(handle);
            if (r)
                handle = IntPtr.Zero;
            return r;
        }

        public bool ExtractFile(string from, string to, OpenFile dwSearchScope)
        {
            var dir = Path.GetDirectoryName(to);

            if (!Directory.Exists(dir) && !String.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);

            return StormLibNet.SFileExtractFile(handle, from, to, dwSearchScope);
        }

        public IntPtr OpenFile(string FileName, OpenFile dwSearchScope)
        {
            IntPtr s;
            StormLibNet.SFileOpenFileEx(this.handle, "Actor\\A1_BlackMushroom.acr", dwSearchScope, out s);
            return s;
        }

        public byte[] ReadFile(IntPtr FileHandle, long FileSize)
        {
            byte[] test = new byte[FileSize];
            long _readbytes;
            var result = StormLibNet.SFileReadFile(FileHandle, test, FileSize, out _readbytes);
            return test;
        }
        public long GetFileSize(IntPtr FileHandle)
        {
            uint high;
            uint low = StormLibNet.SFileGetFileSize(FileHandle, out high);
            return (long)high << 32 | low;
        }

        public IntPtr FindFirstFile(string Mask, ref _SFILE_FIND_DATA FindInfo) // Returns Search Handle
        {
            return StormLibNet.SFileFindFirstFile(this.handle, Mask, ref FindInfo, null);
        }
        public bool FindNextFile(IntPtr SearchHandle, ref _SFILE_FIND_DATA FindInfo)
        {
            return StormLibNet.SFileFindNextFile(SearchHandle, ref FindInfo);
        }

         public bool FindClose(IntPtr SearchHandle)
        {
            return StormLibNet.SFileFindClose(SearchHandle);
        }

        public bool CloseFile(IntPtr FileHandle)
        {
            return StormLibNet.SFileCloseFile(FileHandle);
        }
    }

    public class MpqArchiveSet : IDisposable
    {
        private List<MpqArchive> archives = new List<MpqArchive>();
        private string GameDir = ".\\";

        public void SetGameDir(string dir)
        {
            GameDir = dir;
        }
        public bool CloseFile(IntPtr FileHandle)
        {
            return archives[0].CloseFile(FileHandle);
        }
        public bool FindClose(IntPtr SearchHandle)
        {
            return archives[0].FindClose(SearchHandle);
        }
        public bool FindNextFile(IntPtr SearchHandle, ref _SFILE_FIND_DATA FindInfo)
        {
            return archives[0].FindNextFile(SearchHandle, ref FindInfo);
        }
        public IntPtr FindFirstFile(string Mask, ref _SFILE_FIND_DATA FindInfo)
        {
            return archives[0].FindFirstFile(Mask, ref FindInfo);
        }
        public long GetFileSize(IntPtr FileHandle)
        {
            return archives[0].GetFileSize(FileHandle);
        }
        public static string GetGameDirFromReg()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Blizzard Entertainment\\Diablo III");
            if (key == null)
                return null;
            Object val = key.GetValue("InstallPath");
            if (val == null)
                return null;
            return val.ToString();
        }

        public bool AddArchive(string file)
        {
            Console.WriteLine("Adding archive: {0}", file);

            MpqArchive a = new MpqArchive(GameDir + file, 0, OpenArchiveFlags.READ_ONLY);
            if (a.IsOpen)
            {
                archives.Add(a);
                Console.WriteLine("Added archive: {0}", file);
                return true;
            }
            Console.WriteLine("Failed to add archive: {0}", file);
            return false;
        }

        public int AddArchives(string[] files)
        {
            int n = 0;
            foreach (string s in files)
                if (AddArchive(s))
                    n++;
            return n;
        }
        public IntPtr OpenFile(string file, OpenFile dwSearchScope)
        {
            return archives[0].OpenFile(file, dwSearchScope);
        }
        public byte[] ReadFile(IntPtr filehandle, long filesize)
        {
            return archives[0].ReadFile(filehandle, filesize);
        } 
        public bool ExtractFile(string from, string to, OpenFile dwSearchScope)
        {
            foreach (MpqArchive a in archives)
            {
                var r = a.ExtractFile(from, to, dwSearchScope);
                if (r)
                    return true;
            }
            return false;
        }

        public void Dispose()
        {
            Close();
        }

        public void Close()
        {
            foreach (MpqArchive a in archives)
                a.Close();
            archives.Clear();
        }
    }

 


}
