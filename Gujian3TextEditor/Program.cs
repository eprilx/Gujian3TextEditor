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

namespace Gujian3TextEditor
{
    class Program
    {
        static string inputPath = "";
        static string inputTXT = "";
        static string outputPath = "";
        static string command = "";
        static string ExePath = Process.GetCurrentProcess().ProcessName;
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

            if( args.Length == 0)
            {
                Console.WriteLine("Please run in cmd");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                return;
            }
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
                    v => BsonDataRead.ExtractAll = v != null },
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
                        BsonDataRead.Extract(inputPath, outputPath);
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
                if (command == null)
                    PrintCredit();
                Console.Write("\nUsage: " + ExePath + " ");
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
                        Console.WriteLine(ExePath + " -e -i gujian3_text.bin");
                        break;
                    case "pack":
                        Console.WriteLine("\nExample:");
                        Console.WriteLine(ExePath + " -p -i gujian3_text.bin -t gujian3_text.bin.txt");
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
                Console.Write("\n" + ExePath + " v" + ToolVersion);
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