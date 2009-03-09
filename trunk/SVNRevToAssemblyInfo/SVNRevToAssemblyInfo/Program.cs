using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace SVNRevToAssemblyInfo
{
    class Program
    {
        static void Main(string[] args)
        {
            // args[0] is the trunk folder 
            // args[1] is the file path of AssemblyInfo.cs or AssemblyInfo.vb

            if (args.Length == 2)
            {
                Console.WriteLine("Working folder: " + args[0]);
                LibSubWCRev.SubWCRevClass svn = new LibSubWCRev.SubWCRevClass();
                svn.GetWCInfo(args[0], true, true);
                Console.WriteLine("Latest Revision: " + svn.Revision);
                Console.WriteLine("Last Author: " + svn.Author);
                Console.WriteLine("Last Commited Date: " + svn.Date);
                Console.WriteLine("SVN: " + svn.Url);

                string ai = "";
                using (StreamReader sr = new StreamReader(args[1]))
                {
                    Console.WriteLine("Reading: " + args[1]);
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
                        Console.WriteLine("Writing: " + args[1]);
                        sw.WriteLine(ai);
                        Console.WriteLine(Environment.NewLine);
                        Console.WriteLine(ai);
                    }
                }
            }
        }
    }
}
