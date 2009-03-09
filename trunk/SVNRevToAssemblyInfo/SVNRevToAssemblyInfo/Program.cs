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

                // Open AssemblyInfo for writing
                string ai = "";
                using (StreamReader sr = new StreamReader(args[1]))
                {
                    // Read
                    Console.WriteLine("Reading: " + args[1]);
                    ai = sr.ReadToEnd();
                    // Overwrite the Rev number 
                    ai = Regex.Replace(ai, "9999", svn.Revision.ToString());
                }
                using (StreamWriter sw = new StreamWriter(args[1]))
                {
                    // Write new file
                    if (!string.IsNullOrEmpty(ai))
                    {
                        sw.WriteLine(ai);
                    }
                }
            }
        }
    }
}
