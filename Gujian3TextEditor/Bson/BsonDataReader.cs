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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Gibbed.IO;

namespace Gujian3TextEditor.Bson
{
    class BsonDataReader
    {
        private static FileStream input;
        private static StreamWriter outputTxt;
        internal static BsonType type;
        private static List<string> tagList = Ulities.getTagList();
        private static string tag = "";
        private static uint countArray;
        private static long offset;
        public static bool ExtractAll = false;

        internal static void Extract(string PathIn, string PathOut)
        {
            input = File.OpenRead(PathIn);
            outputTxt = File.CreateText(PathOut);
            if ((tagList != null) && (!tagList.Any()))
            {
                ExtractAll = true;
            }
            ReadHeader();
            //input.Position = 25680567;
            while (input.Position < input.Length)
            {
                type = (BsonType)input.ReadValueS8();
                ReadType(type);
            }
            outputTxt.Close();
        }

        static void ReadHeader()
        {
            input.Position = 0;
            var magic = input.ReadValueU32();
            if (magic != 0xF2060881 && magic != 0xD5B50981)
            {
                //throw new Exception("INVALID HEADER/VERSION");
            }
        }
        internal static void ReadType(BsonType type)
        {
            int sizeString;
            if ((type != BsonType.ByteString && type != BsonType.UShortString && type != BsonType.Array))
            {
                tag = "";
            }
            offset = input.Position - 1;
            switch (type)
            {
                case BsonType.ByteString:
                    sizeString = input.ReadValueU8();
                    ReadString(sizeString, offset);
                    break;
                case BsonType.UShortString:
                    sizeString = input.ReadValueU16();
                    ReadString(sizeString, offset);
                    break;
                case BsonType.Unk0:
                    break;
                case BsonType.Unk1:
                    break;
                case BsonType.Unk2:
                    break;
                case BsonType.SByte:
                    input.ReadValueS8();
                    break;
                case BsonType.Short:
                    input.ReadValueS16();
                    break;
                case BsonType.Int:
                    input.ReadValueS32();
                    break;
                case BsonType.Long:
                    input.ReadValueS64();
                    break;
                case BsonType.Float:
                    input.ReadValueF64();
                    break;
                case BsonType.Array:
                    countArray = input.ReadValueU32();
                    if (countArray == 0)
                    {
                        tag = "";
                    }
                    for (int i = 0; i < countArray; i++)
                    {
                        type = (BsonType)input.ReadValueS8();
                        ReadType(type);
                    }
                    break;
                case BsonType.Unk13:
                    break;
                case BsonType.UShort:
                    input.ReadValueU16();
                    break;
                case BsonType.UInt:
                    input.ReadValueU32();
                    break;
                default:
                    throw new Exception("INVALID TYPE AT OFFSET = " + offset + ", TYPE = " + type);
            }
        }

        static void ReadString(int len, long offset)
        {
            string text = input.ReadString(len);
            if (ExtractAll == true)
            {
                if (tag == "")
                    tag = "<NoTag>";
                if (text != "")
                {
                    text = text.Replace("\n", "<n>");
                    text = text.Replace("\r", "<r>");
                    outputTxt.WriteLine("{0},{1},{2}", offset, tag, text);
                }
                else
                {
                    outputTxt.WriteLine("{0},{1},{2}", offset, tag, "<NoText>");
                }
            }
            else if (tagList.Contains(tag) && tag != "")
            {
                if (text != "")
                {
                    text = text.Replace("\n", "<n>");
                    text = text.Replace("\r", "<r>");
                    outputTxt.WriteLine("{0},{1},{2}", offset, tag, text);
                }
            }

            if (tagList.Contains(text))
            {
                tag = text;
            }
            else
            {
                if (type != BsonType.Array)
                {
                    tag = "";
                }
            }
        }
    }
}
