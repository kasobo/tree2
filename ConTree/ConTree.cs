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
                if (arg == "/?")
                {
                    ShowUsage();
                    return 0;
                }
                else if (arg == "/F")
                    showFiles = true;
                else if (arg == "/A")
                    showing = OutputType.Ascii;
                else if (arg == "/W")
                    showing = OutputType.Web;
                else if (arg == "/2")
                    tab = 2;
                else if (arg.StartsWith ("/"))
                { Console.WriteLine ("Invalid switch - " + args[ix]); return 1; }
                else if (rootPath != null)
                { Console.WriteLine ("Too many parameters - " + args[ix]); return 2; }
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


        static void ShowUsage()
        {
            string exe = System.Diagnostics.Process.GetCurrentProcess().ProcessName;

            Console.WriteLine ("Graphically displays the folder structure of a drive or path.");
            Console.WriteLine ();
            Console.WriteLine (exe + " [drive:][path] [/F] [/A] [/W] [/2]");
            Console.WriteLine ();
            Console.WriteLine ("   /F   Display the names of the files in each folder.");
            Console.WriteLine ("   /A   Use ASCII instead of extended characters.");
            Console.WriteLine ("   /W   Produce output suitable for a static HTML web page.");
            Console.WriteLine ("   /2   Indent by 2 instead of 4.");
        }
    }
}
