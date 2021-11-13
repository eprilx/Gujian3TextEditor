/*
MIT License

Copyright (c) 2021 eprilx

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

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
