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

namespace Gujian3TextEditor
{
    class test
    {
        private struct LineStruct
        {
            internal uint offset;
            internal string tag;
            internal string text;
        }
        private static List<string> tagList = Ulities.getTagList();
        internal static void TestChinese(string PathTXT)
        {

            StreamReader inputTxt = File.OpenText(PathTXT);

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
            var outputChinese = File.CreateText("chinese.txt");
            Parallel.For(2284295, listLine.Count, i =>
            {
               
                if (!tagList.Contains(listLine[i - 1].text))
                {
                    if (Ulities.ContainsUnicodeCharacter(listLine[i].text) == true)
                    {
                        outputChinese.WriteLine(lines.ElementAt(i - 1) + "\r\n" + lines.ElementAt(i));
                    }
                }
            });
            outputChinese.Close();
        }
    }
}