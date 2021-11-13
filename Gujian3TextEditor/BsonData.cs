using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gibbed.IO;

namespace Gujian3TextEditor
{
    public enum BsonType : sbyte
    {
        Unk0 = 0,
        Unk1 = 1,
        Unk2 = 2,
        Byte = 3,
        Short = 4,
        Int = 5,
        Long = 6,
        Float = 7,
        ByteString = 8,
        ShortString = 9,
        Array = 11,
        Unk13 = 13,
        UShort = 15,
        UInt = 16
    }
    class BsonDataRead
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
            if (magic != 0xF2060881)
            {
                throw new Exception("INVALID HEADER");
            }
        }
        internal static void ReadType(BsonType type)
        {
            int sizeString;
            if ((type != BsonType.ByteString && type != BsonType.ShortString && type != BsonType.Array))
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
                case BsonType.ShortString:
                    sizeString = input.ReadValueU16();
                    ReadString(sizeString, offset);
                    break;
                case BsonType.Unk0:
                    break;
                case BsonType.Unk1:
                    break;
                case BsonType.Unk2:
                    break;
                case BsonType.Byte:
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
                    if(countArray == 0)
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

            if(tagList.Contains(text))
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
    class BsonDataPackString
    {
        
        private static FileStream inputOri;
        private static StreamReader inputTxt;
        private static FileStream outputNew;
        private struct LineStruct
        {
            internal uint offset;
            internal string tag;
            internal string text;
        }
        internal static void Pack(string PathOri, string PathTXT, string PathNew)
        {
            //if (PathTXT == "")
            //{
            //    PathTXT = PathOri + ".txt";
            //}
            //if (PathNew == "")
            //{
            //    PathNew = PathOri + ".new";
            //}
            inputOri = File.OpenRead(PathOri);
            inputTxt = File.OpenText(PathTXT);
            outputNew = File.Create(PathNew);

            var lines = File.ReadLines(PathTXT);
            // read text
            List<LineStruct> listLine = new();
            foreach (string line in lines)
            {
                string[] splits = line.Split(new[] { ',' }, 3);

                uint offset = uint.Parse(splits[0]);
                string tag = splits[1];
                string text = splits[2];
                listLine.Add(new LineStruct
                {
                    offset = uint.Parse(splits[0]),
                    tag = splits[1],
                    text = splits[2]
                });
            }
            // sort by offset
            listLine.Sort((s1, s2) => s1.offset.CompareTo(s2.offset));
            listLine = listLine.ToList();
            // write
            foreach (LineStruct line in listLine)
            {
                Ulities.CopyBuffer(outputNew, inputOri, (line.offset - inputOri.Position));

                switch ((BsonType)inputOri.ReadValueS8())
                {
                    case BsonType.ByteString:
                        inputOri.ReadBytes(inputOri.ReadValueU8());
                        break;
                    case BsonType.ShortString:
                        inputOri.ReadBytes(inputOri.ReadValueU16());
                        break;
                }
                WriteString(line.text);
            }

            Ulities.CopyBuffer(outputNew, inputOri, (inputOri.Length - inputOri.Position));
            outputNew.Close();
        }
        static void WriteString(string text)
        {

            text = text.Replace("<n>", "\n");
            text = text.Replace("<r>", "\r");
            text = text.Replace("<NoText>", "");
            byte[] bytesText = Encoding.UTF8.GetBytes(text);
            if(bytesText.Length > (uint)0xFFFF)
            {
                throw new Exception("INVALID LENGTH STRING TEXT = " + text);
            }
            if (bytesText.Length < 256)
            {
                outputNew.WriteByte(0x08);
                outputNew.WriteValueU8((byte)bytesText.Length);
            }
            else
            {
                outputNew.WriteByte(0x09);
                outputNew.WriteValueU16((ushort)bytesText.Length);
            }
            outputNew.WriteBytes(bytesText);
        }
    }
}
