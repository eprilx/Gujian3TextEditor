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
    }
}
