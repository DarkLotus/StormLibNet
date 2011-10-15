using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StormLibNet;

namespace StormLibTest
{
    class Program
    {
        static readonly MpqArchiveSet archive = new MpqArchiveSet();
        static void Main(string[] args)
        {
            archive.SetGameDir("C:\\Program Files (x86)\\Diablo III Beta\\Data_D3\\PC\\MPQs\\");
            archive.AddArchive("CoreData.mpq");
            var ra = archive.OpenFile("A1_BlackMushroom.acr", OpenFile.FROM_MPQ);
            var filesize = archive.GetFileSize(ra);
            var xa = archive.ReadFile(ra,filesize);
            _SFILE_FIND_DATA fileinfo = new _SFILE_FIND_DATA();
            var temp = archive.FindFirstFile("Actor\\*", ref fileinfo);

            List<string> filelist = new List<string>();
            while (archive.FindNextFile(temp, ref fileinfo))
            {
                filelist.Add(fileinfo.cFileName);
            }
        }
    }
}
