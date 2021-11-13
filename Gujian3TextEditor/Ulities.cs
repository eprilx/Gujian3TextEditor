using Gibbed.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gujian3TextEditor
{
    class Ulities
    {
        public static List<string> getTagList()
        {
            List<string> tagList = new();
            try
            {
                var lines = File.ReadLines("taglist.lst");
                foreach(string line in lines)
                {
                    tagList.Add(line);
                }
            }
            catch
            {

            }
            return tagList;
        }
        public static void CopyBuffer(FileStream @out, FileStream @in, long length, long offset)
        {
            @in.Position = offset;
            long count = length / 0x1000;
            int remainder = (int)(length % 0x1000);
            for (uint i = 0; i < count; i++)
            {
                @out.WriteBytes(@in.ReadBytes(0x1000));
            }
            @out.WriteBytes(@in.ReadBytes(remainder));
        }
        public static void CopyBuffer(FileStream @out, FileStream @in, long length)
        {
            long count = length / 0x1000;
            int remainder = (int)(length % 0x1000);
            for (uint i = 0; i < count; i++)
            {
                @out.WriteBytes(@in.ReadBytes(0x1000));
            }
            @out.WriteBytes(@in.ReadBytes(remainder));
        }
    }
}
