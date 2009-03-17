using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using CommandLineParserLib;
using System.Reflection;
using SharpSvn.Implementation;
using SharpSvn; 

namespace SVNRevToAssemblyInfo
{
    class Program
    {
        const string FLAG_WORKING_DIR = "dir";
        const string FLAG_ASSEMBLY_INFO = "file";

        static string GetCommandLine()
        {
            string cli = Environment.CommandLine;
            cli = cli.Replace(Path.GetFileName(Assembly.GetExecutingAssembly().Location), "").Trim();
            cli = cli.Replace("\"" + Path.GetFileName(Assembly.GetExecutingAssembly().Location) + "\"", "").Trim();
            cli = cli.Replace("\"" + Assembly.GetExecutingAssembly().Location + "\"", "").Trim();
            cli = cli.Replace("\"" + Assembly.GetExecutingAssembly().Location.Replace(".exe", ".vshost.exe") + "\"", "").Trim();

            return cli;
        }
        static void Main(string[] args)
        {
            // args[0] is the trunk folder 
            // args[1] is the file path of AssemblyInfo.cs or AssemblyInfo.vb

            string pWorkingdir = "";
            string pAssemblyInfo = "";

            StringBuilder sbDebug = new StringBuilder();

            CommandLineParser parser = new CommandLineParser();
            parser.CommandLine = GetCommandLine();
            sbDebug.AppendLine("Command Line: " + parser.CommandLine);

            CommandLineEntry dirArg = parser.CreateEntry(CommandTypeEnum.Flag, FLAG_WORKING_DIR);
            parser.Entries.Add(dirArg);
            CommandLineEntry dirVal = dirArg;
            dirArg = parser.CreateEntry(CommandTypeEnum.ExistingFolder);
            dirArg.MustFollowEntry = dirVal;
            parser.Entries.Add(dirArg);

            CommandLineEntry fileArg = parser.CreateEntry(CommandTypeEnum.Flag, FLAG_ASSEMBLY_INFO);
            parser.Entries.Add(fileArg);
            CommandLineEntry fileVal = fileArg;
            fileArg = parser.CreateEntry(CommandTypeEnum.ExistingFile);
            fileArg.MustFollowEntry = fileVal;
            parser.Entries.Add(fileArg);

            if (parser.Parse())
            {
                for (int i = 0; i < parser.Entries.Count; i++)
                {
                    CommandLineEntry entry = parser.Entries.get_Item(i);
                    if (entry.Value.Equals(FLAG_WORKING_DIR))
                    {
                        if (parser.Entries.Count > 1)
                        {
                            pWorkingdir = parser.Entries.get_Item(++i).Value;
                        }
                    }
                    else if (entry.Value.Equals(FLAG_ASSEMBLY_INFO))
                    {
                        if (parser.Entries.Count > 1)
                        {
                            pAssemblyInfo = parser.Entries.get_Item(++i).Value;
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(pWorkingdir) && !string.IsNullOrEmpty(pAssemblyInfo))
            {
                sbDebug.AppendLine("Working folder: " + pWorkingdir);

                SvnClient sc = new SvnClient();
                SharpSvn.SvnTarget st = SvnTarget.FromString(pWorkingdir);
                SvnInfoEventArgs svnInfo;
                sc.GetInfo(st, out svnInfo);
                Console.WriteLine(svnInfo.Revision);
                                                
                sbDebug.AppendLine("Latest Revision: " + svnInfo.Revision);
                sbDebug.AppendLine("Last Author: " + svnInfo.LastChangeAuthor);
                sbDebug.AppendLine("Last Commited Date: " + svnInfo.LastChangeTime);
                sbDebug.AppendLine("SVN: " + svnInfo.Uri);

                string ai = "";
                using (StreamReader sr = new StreamReader(pAssemblyInfo))
                {
                    sbDebug.AppendLine("Reading: " + pAssemblyInfo);
                    ai = sr.ReadToEnd();
                    string version = "AssemblyVersion"; //AssemblyFileVersion
                    int rev = 0;
                    int.TryParse(svnInfo.Revision.ToString(), out rev);
                    rev++;
                    ai = Regex.Replace(ai, "(?<=" + version + "\\(\"\\d+\\.\\d+.\\d+\\.)\\d+(?=\"\\)])", rev.ToString());
                }
                if (!string.IsNullOrEmpty(ai))
                {
                    using (StreamWriter sw = new StreamWriter(pAssemblyInfo))
                    {
                        sbDebug.AppendLine("Writing: " + pAssemblyInfo);
                        sw.WriteLine(ai);
                        sbDebug.AppendLine(Environment.NewLine);
                        sbDebug.Append(ai);
                    }
                }
            }

            Console.WriteLine(sbDebug.ToString());

            using (StreamWriter sw = new StreamWriter("debug.log"))
            {
                sw.WriteLine(sbDebug.ToString());
            }

        }
    }
}
