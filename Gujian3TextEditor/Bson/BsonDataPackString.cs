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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gibbed.IO;

namespace Gujian3TextEditor.Bson
{
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
            inputOri = File.OpenRead(PathOri);
            inputTxt = File.OpenText(PathTXT);
            outputNew = File.Create(PathNew);

            var lines = File.ReadLines(PathTXT);
            // read text
            List<LineStruct> listLine = new();
            foreach (string line in lines)
            {
                string[] splits = line.Split(new[] { ',' }, 3);

                listLine.Add(new LineStruct
                {
                    offset = uint.Parse(splits[0]),
                    tag = splits[1],
                    text = splits[2]
                });
            }
            var outputChinese = File.CreateText("chinese2.txt");
            Parallel.For(2284295, listLine.Count, i =>
            {
                if (listLine[i - 1].text != "prop_base_name" && listLine[i - 1].text != "prop_npc_billboard_name")
                {
                    if (Ulities.ContainsUnicodeCharacter(listLine[i].text) == true)
                    {
                        outputChinese.WriteLine(lines.ElementAt(i - 1));
                        outputChinese.WriteLine(lines.ElementAt(i));
                    }
                }
            });
            outputChinese.Close();
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
                    case BsonType.UShortString:
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
            if (bytesText.Length > (uint)0xFFFF)
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
