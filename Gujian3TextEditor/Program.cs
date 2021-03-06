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

using Mono.Options;
using System;
using System.Text;
using System.IO;
using Gibbed.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Globalization;
using System.Threading;
using System.Diagnostics;
using Gujian3TextEditor.Bson;

namespace Gujian3TextEditor
{
    class Program
    {
        static string inputPath = "";
        static string inputTXT = "";
        static string outputPath = "";
        static string command = "";
        static string ExeName = Process.GetCurrentProcess().ProcessName;
        static void Main(string[] args)
        {
            string ToolVersion;
            try
            {
                ToolVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                ToolVersion = ToolVersion.Remove(ToolVersion.Length - 2);
            }
            catch
            {
                ToolVersion = "1.0.0";
            }

            // Change current culture
            CultureInfo culture;
            culture = CultureInfo.CreateSpecificCulture("en-US");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            StreamHelpers.DefaultEncoding = Encoding.UTF8;

            var p = new OptionSet()
            {
                {"e|extract", "Extract String",
                v => {command = "extract"; } },
                {"p|pack", "Pack String",
                v=> {command = "pack"; } }
            };
            p.Parse(args);

            switch (command)
            {
                case "extract":
                    p = new OptionSet() {
                {"a|all","(optional) Extract all strings",
                    v => BsonDataReader.ExtractAll = v != null },
                { "i|input=", "(required) Decrypted text buffer path",
                    v => inputPath = v },
                { "o|output=", "(optional) Output text file path",
                    v => outputPath = v }
                };
                    break;
                case "pack":
                    p = new OptionSet() {
                {"i|input=", "(required) Decrypted text buffer path",
                    v => inputPath = v },
                {"t|text=", "(required) Text file path",
                    v => inputTXT = v},
                {"o|output=",  "(optional) Output file path",
                    v => outputPath = v }
                };
                    break;
            }
            p.Parse(args);

            if (args.Length == 0 || inputPath == "" || (inputTXT == "" && command == "pack"))
            {
                ShowHelp(p);
                return;
            }

            try
            {
                switch (command)
                {
                    case "extract":
                        if (outputPath == "")
                            outputPath = inputPath + ".txt";
                        BsonDataReader.Extract(inputPath, outputPath);
                        break;
                    case "pack":
                        if (outputPath == "")
                            outputPath = inputPath + ".new";
                        BsonDataPackString.Pack(inputPath, inputTXT, outputPath);
                        break;
                }
                Done(outputPath);
            }
            finally
            {
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                Environment.Exit(0);
            }

            void ShowHelp(OptionSet p)
            {
                if (command == "")
                    PrintCredit();
                Console.Write("\nUsage: " + ExeName + " ");
                switch (command)
                {
                    case "extract":
                        Console.WriteLine("--extract [OPTIONS]");
                        break;
                    case "pack":
                        Console.WriteLine("--pack [OPTIONS]");
                        break;
                    default:
                        Console.WriteLine("[OPTIONS]");
                        break;
                }

                Console.WriteLine("Options:");
                p.WriteOptionDescriptions(Console.Out);

                switch (command)
                {
                    case "extract":
                        Console.WriteLine("\nExample:");
                        Console.WriteLine(ExeName + " -e -i gujian3_text.bin");
                        break;
                    case "pack":
                        Console.WriteLine("\nExample:");
                        Console.WriteLine(ExeName + " -p -i gujian3_text.bin -t gujian3_text.bin.txt");
                        break;
                }
                //if (command == "")
                //{
                //    Console.WriteLine("\nExample:");
                //    Console.WriteLine(ExePath + " --extract --all -p gujian3_text.bin");
                //    Console.WriteLine(ExePath + " --pack -p gujian3_text.bin -t gujian3_text.bin.txt");
                //    //Console.WriteLine("\nMore usage: https://github.com/eprilx/");
                //    //Console.Write("More update: ");
                //    //Console.WriteLine("https://github.com/eprilx/");
                //}
            }


            void PrintCredit()
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("\n" + ExeName + " v" + ToolVersion);
                Console.WriteLine(" by eprilx");
                Console.Write("Special thanks to: ");
                Console.WriteLine("alanm, Kaplas");
                Console.ResetColor();
            }

            void Done(string output)
            {
                Console.Write("\n********************************************");
                PrintCredit();
                Console.WriteLine("********************************************");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("\n" + output + " has been created!");
                Console.ResetColor();
            }
        }
    }
}