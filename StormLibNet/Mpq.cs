using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace StormLibNet
{
    public static class Mpq
    {
        static readonly string[] archiveNames = {
                    "CoreData.mpq",
                    "ClientData.mpq",
 };
        static readonly MpqArchiveSet archive = new MpqArchiveSet();
        static readonly string regGameDir = MpqArchiveSet.GetGameDirFromReg();

        static Mpq()
        {
            var dir = Path.Combine(regGameDir, "Data_D3\\PC\\MPQs\\");
           archive.SetGameDir(dir);

            //Console.WriteLine("Game dir is {0}", );

            archive.AddArchives(archiveNames);
        }

        public static bool ExtractFile(string from, string to, OpenFile dwSearchScope)
        {
            return archive.ExtractFile(from, to, dwSearchScope);
        }
        public static IntPtr OpenFile(string filename, OpenFile dwSearchScope)
        {
            return archive.OpenFile(filename, dwSearchScope);
        }
        public static void Close()
        {
            archive.Close();
        }
    }
}
