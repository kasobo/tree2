using System;
using System.IO;
using FullSys;

namespace AppMain
{
    enum OutputType { Ascii, Lines, Web };

    static class ConTreeMain
    {
        static bool showFiles = false;
        static OutputType showing = OutputType.Lines;

        static int tab = 4;
        static string rootPath = null;

        static int Main (string[] args)
        {
            string err = null;

            for (int ix = 0; ix < args.Length; ++ix)
            {
                var arg = args[ix].ToUpper();
                if (arg == "/F")
                    showFiles = true;
                else if (arg == "/A")
                    showing = OutputType.Ascii;
                else if (arg == "/W")
                    showing = OutputType.Web;
                else if (arg == "/2")
                    tab = 2;
                else if (arg.StartsWith ("/"))
                { Console.WriteLine ("Invalid switch - " + arg); return 1; }
                else if (ix != args.Length-1)
                { Console.WriteLine ("Too many parameters - " + arg); return 2; }
                else
                    rootPath = arg;
            }

            if (rootPath == null)
                rootPath = @".";

            try
            {
                if (showing == OutputType.Ascii)
                    foreach (DirTreeLocation node in new DirTreeScanner (rootPath, tab))
                        node.WriteNodeAsText (showFiles);
                else if (showing == OutputType.Web)
                {
                    foreach (DirTreeLocation437 node in new DirTreeScanner437 (rootPath, tab))
                        node.WriteNodeAsHtml (showFiles);
                    DirTreeLocation437.WriteHtmlTrailer();
                }
                else
                    foreach (DirTreeLocation node in new DirTreeScanner437 (rootPath, tab))
                        node.WriteNodeAsText (showFiles);
            }
            catch (IOException ex)
            { err = ex.Message.Trim(); }
            catch (UnauthorizedAccessException ex)
            { err = ex.Message.Trim(); }

            if (err != null)
            {
                Console.WriteLine (err);
                return 1;
            }

            return 0;
        }
    }
}
