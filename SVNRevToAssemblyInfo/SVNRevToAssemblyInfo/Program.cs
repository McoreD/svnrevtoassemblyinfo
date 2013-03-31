using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using CommandLineParserLib;
using SharpSvn;
using SharpSvn.Implementation;

namespace SVNRevToAssemblyInfo
{
    internal class Program
    {
        static StringBuilder sbDebug = new StringBuilder();

        const string FLAG_WORKING_DIR = "dir";
        const string FLAG_ASSEMBLY_INFO = "file";
        const string FLAG_AUTO = "auto";
        const string FLAG_UPDATE = "update";

        private static string GetCommandLine()
        {
            string cli = Environment.CommandLine;
            cli = cli.Replace(Path.GetFileName(Assembly.GetExecutingAssembly().Location), "").Trim();
            cli = cli.Replace("\"" + Path.GetFileName(Assembly.GetExecutingAssembly().Location) + "\"", "").Trim();
            cli = cli.Replace("\"" + Assembly.GetExecutingAssembly().Location + "\"", "").Trim();
            cli = cli.Replace("\"" + Assembly.GetExecutingAssembly().Location.Replace(".exe", ".vshost.exe") + "\"", "").Trim();

            return cli;
        }

        private static void Main(string[] args)
        {
            // args[0] is the trunk folder
            // args[1] is the file path of AssemblyInfo.cs or AssemblyInfo.vb

            string pWorkingdir = "";
            string pAssemblyInfo = "";
            bool bAuto = false;
            bool bUpdate = false;

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

            parser.Entries.Add(parser.CreateEntry(CommandTypeEnum.Flag, FLAG_AUTO));
            parser.Entries.Add(parser.CreateEntry(CommandTypeEnum.Flag, FLAG_UPDATE));

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
                    else if (entry.Value.Equals(FLAG_AUTO))
                    {
                        bAuto = true;
                    }
                    else if (entry.Value.Equals(FLAG_UPDATE))
                    {
                        bUpdate = true;
                    }
                }
            }

            if (!string.IsNullOrEmpty(pWorkingdir))
            {
                sbDebug.AppendLine("Working folder: " + pWorkingdir);

                SvnClient sc = new SvnClient();
                SharpSvn.SvnTarget st = SvnTarget.FromString(pWorkingdir);
                SvnInfoEventArgs svnInfo;
                sc.GetInfo(st, out svnInfo);
                Console.WriteLine("Current Revision: " + svnInfo.Revision);

                if (bUpdate)
                {
                    Console.WriteLine("Updating SVN");
                    sc.Update(pWorkingdir);
                    sc.GetInfo(st, out svnInfo);
                }

                sbDebug.AppendLine("Latest Revision: " + svnInfo.Revision);
                sbDebug.AppendLine("Last Author: " + svnInfo.LastChangeAuthor);
                sbDebug.AppendLine("Last Commited Date: " + svnInfo.LastChangeTime);
                sbDebug.AppendLine("SVN: " + svnInfo.Uri);

                if (File.Exists(pAssemblyInfo))
                {
                    UpdateFile(svnInfo.Revision.ToString(), pAssemblyInfo);
                }
                else if (bAuto)
                {
                    List<string> files = new List<string>();
                    files.AddRange(Directory.GetFiles(pWorkingdir, "AssemblyInfo*.cs", SearchOption.AllDirectories));
                    foreach (string cs in files)
                    {
                        UpdateFile(svnInfo.Revision.ToString(), cs);
                    }
                }
            }

            Console.WriteLine(sbDebug.ToString());
        }

        private static void UpdateFile(string svnVersion, string pAssemblyInfo)
        {
            string ai = "";
            using (StreamReader sr = new StreamReader(pAssemblyInfo))
            {
                ai = sr.ReadToEnd();
                string version = "AssemblyVersion"; //AssemblyFileVersion
                int rev = 0;
                int.TryParse(svnVersion, out rev);
                rev++;
                ai = Regex.Replace(ai, "(?<=" + version + "\\(\"\\d+\\.\\d+.\\d+\\.)\\d+(?=\"\\)])", rev.ToString());
            }
            if (!string.IsNullOrEmpty(ai))
            {
                using (StreamWriter sw = new StreamWriter(pAssemblyInfo))
                {
                    sbDebug.AppendLine("Writing: " + pAssemblyInfo);
                    sw.Write(ai);
                }
            }
        }
    }
}