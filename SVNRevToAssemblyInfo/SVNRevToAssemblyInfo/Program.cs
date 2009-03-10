using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using NDesk.Options;

namespace SVNRevToAssemblyInfo
{
    class Program
    {

        static void Main(string[] args)
        {
            // args[0] is the trunk folder 
            // args[1] is the file path of AssemblyInfo.cs or AssemblyInfo.vb

            StringBuilder sbDebug = new StringBuilder();
            sbDebug.AppendLine("Command Line: " + Environment.CommandLine);

            bool bShowHelp = false;
            string pWorkingdir = "";
            string fAssemblyInfo = "";
            bool bWriteDebug = false;
   
            var p = new OptionSet() {
                { "i|input=", "Working folder path such as the trunk folder.",
                  (string v) => pWorkingdir = v },
                { "o|output=", "Assemblyinfo file path.",
                  (string v) => fAssemblyInfo = v },
                { "v|verbose", "Verbose Mode",
                  v => { if (v != null) bWriteDebug = true; } },
                { "h|help",  "show this message and exit", 
                  v => bShowHelp = v != null },
            };

            List<string> extra;
            try
            {
                extra = p.Parse(args);
            }
            catch (OptionException e)
            {
                Console.Write("SVNRevToAssembyInfo: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Try SVNRevToAssembyInfo.exe --help' for more information.");
                return;
            }

            if (bShowHelp)
            {
                ShowHelp(p);
                return;
            }

            if (extra.Count > 0)
            {
                for (int i = 0; i < extra.Count; i++)
                {
                    sbDebug.AppendLine(string.Format("Arg {0} {1}", i, extra[i]));
                }
            }

            sbDebug.AppendLine(Environment.NewLine);

            if (args.Length == 2)
            {
                sbDebug.AppendLine("Working folder: " + args[0]);
                LibSubWCRev.SubWCRevClass svn = new LibSubWCRev.SubWCRevClass();
                svn.GetWCInfo(args[0], true, true);
                sbDebug.AppendLine("Latest Revision: " + svn.Revision);
                sbDebug.AppendLine("Last Author: " + svn.Author);
                sbDebug.AppendLine("Last Commited Date: " + svn.Date);
                sbDebug.AppendLine("SVN: " + svn.Url);

                string ai = "";
                using (StreamReader sr = new StreamReader(args[1]))
                {
                    sbDebug.AppendLine("Reading: " + args[1]);
                    ai = sr.ReadToEnd();
                    string version = "AssemblyVersion"; //AssemblyFileVersion
                    int rev = 0;
                    int.TryParse(svn.Revision.ToString(), out rev);
                    rev++;
                    ai = Regex.Replace(ai, "(?<=" + version + "\\(\"\\d+\\.\\d+.\\d+\\.)\\d+(?=\"\\)])", rev.ToString());
                }
                if (!string.IsNullOrEmpty(ai))
                {
                    using (StreamWriter sw = new StreamWriter(args[1]))
                    {
                        sbDebug.AppendLine("Writing: " + args[1]);
                        sw.WriteLine(ai);
                        sbDebug.AppendLine(Environment.NewLine);
                        sbDebug.AppendLine(ai);
                    }
                }
            }

            using (StreamWriter sw = new StreamWriter("debug.log"))
            {
                sw.WriteLine(sbDebug.ToString());
            }

        }

        static void ShowHelp(OptionSet p)
        {
            Console.WriteLine("Usage: greet [OPTIONS]+ message");
            Console.WriteLine("Greet a list of individuals with an optional message.");
            Console.WriteLine("If no message is specified, a generic greeting is used.");
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
        }

        //static void Debug(string format, params object[] args)
        //{
        //    if (verbosity > 0)
        //    {
        //        Console.Write("# ");
        //        Console.WriteLine(format, args);
        //    }
        //}
    }
}
