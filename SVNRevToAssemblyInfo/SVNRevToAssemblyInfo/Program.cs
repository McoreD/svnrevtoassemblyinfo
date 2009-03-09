using System;
using System.Collections.Generic;
using System.Text;

namespace SVNRevToAssemblyInfo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Working folder: " + args[0]);
            LibSubWCRev.SubWCRevClass svn = new LibSubWCRev.SubWCRevClass();
            svn.GetWCInfo(args[0], true, true);
            Console.WriteLine("Latest Revision: " + svn.Revision);
            Console.WriteLine("Last Author: " + svn.Author);
            Console.WriteLine("Last Commited Date: " + svn.Date);
            Console.WriteLine("SVN: " + svn.Url);
          
            Console.ReadLine();
        }
    }
}
