using System;
using System.Text;
using System.IO;
using Gibbed.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Globalization;
using System.Threading;

namespace Gujian3TextEditor
{
    class Program
    {

        
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

            StreamHelpers.DefaultEncoding = Encoding.UTF8;
            // Change current culture
            CultureInfo culture;
            culture = CultureInfo.CreateSpecificCulture("en-US");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            if (args.Length > 0)
            {
                //string input = args[0];
                if (args[0] == "-e")
                {
                    string input = args[1];
                    var output = input + ".txt";
                    BsonData.Extract(input, output);
                    Done(output);
                }
                if(args[0] == "-i")
                {
                    string PathOri = args[1];
                    string PathTXT = args[2];
                    string PathNew = args[3];
                    BsonDataPack.Pack(PathOri, PathTXT, PathNew);
                    Done(PathNew);
                }
                if (args[0].EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                {
                    string PathOri = args[0].Remove(args[0].Length - 4);
                    string PathTXT = args[0];
                    string PathNew = PathOri + ".new";
                    BsonDataPack.Pack(PathOri, PathTXT, PathNew);
                    Done(PathNew);
                }
                else
                {
                    string input = args[0];
                    var output = input + ".txt";
                    BsonData.Extract(input, output);
                    Done(output);
                }
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
            else
            {

            }

            void PrintCredit()
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("\n" + System.Diagnostics.Process.GetCurrentProcess().ProcessName + " v" + ToolVersion);
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