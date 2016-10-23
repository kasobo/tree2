using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FullSys
{
    public class DirTreeLocation437 : DirTreeLocation
    {
        public DirTreeLocation437 (string rootPath, int tabSize = 4) : base (rootPath, tabSize)
        { }

        public DirTreeLocation437 (DirectoryInfo rootInfo, int tabSize = 4) : base (rootInfo, tabSize)
        { }

        public override string DrawUpDown { get { return "\u2502"; } }
        public override string DrawLeftRight { get { return "\u2500"; } }
        public override string DrawUpRight { get { ++Num; return "\u2514"; } }
        public override string DrawUpDownRight { get { ++Num; return "\u251C"; } }

        public static void WriteHtmlHeader (string title)
        {
            Console.WriteLine ("<!DOCTYPE html>");
            Console.WriteLine ("<html>");
            Console.WriteLine ("<head>");
            Console.Write ("<title>");
            Console.Write (new StringBuilder().AppendHtml (title).ToString());
            Console.WriteLine ("</title>");
            Console.WriteLine ("<meta charset=\"UTF-8\">");
            Console.WriteLine ("<style>");
            Console.WriteLine ("  button.bn { border-width:1px; padding:0px 2px; font-family:monospace; font-size:xx-small; color: red; background-color:black; border-color:red; }");
            Console.WriteLine ("  div.s1 { display:block; }");
            Console.WriteLine ("  div.s2 { display:none; }");
            Console.WriteLine ("</style>");
            Console.WriteLine ("<script type=\"text/javascript\">");
            Console.WriteLine ("function tgl(btn,divName)");
            Console.WriteLine ("{");
            Console.WriteLine ("  var divId = document.getElementById(divName);");
            Console.WriteLine ("  if (divId.className == \"s1\")");
            Console.WriteLine ("  { divId.className = 's2'; btn.textContent = \"+\"; }");
            Console.WriteLine ("  else");
            Console.WriteLine ("  { divId.className = 's1'; btn.textContent = \"-\"; }");
            Console.WriteLine ("}");
            Console.WriteLine ("</script>");
            Console.WriteLine ("</head>");
            Console.WriteLine ("<body style='color:orange; background-color:black; font-family:monospace; font-size:medium; white-space:pre;'>");
        }

        public static void WriteHtmlTrailer()
        {
            Console.WriteLine ("</body>");
            Console.WriteLine ("</html>");
        }

        public void WriteNodeAsHtml (bool showFiles)
        {
            bool hasSubdirsOrFiles = PregetContents (showFiles);
            DirLocation top = items[Depth];

            if (Depth == 0)
            {
                WriteHtmlHeader (RootPath);
                Console.WriteLine (new StringBuilder().AppendHtml (top.Path));
            }
            else if (! hasSubdirsOrFiles)
            {
                Console.Write (GetIndent().ToString());
                Console.Write ("<button class='bn'> </button>");
                Console.WriteLine (new StringBuilder().AppendHtml (top.Name));
            }
            else
            {
                Console.Write (GetIndent().ToString());
                Console.Write ("<button id='b");
                Console.Write (Num);
                Console.Write ("' class='bn' onclick=\"tgl(this,'d");
                Console.Write (Num);
                Console.Write ("')\">+</button>");
                Console.WriteLine (new StringBuilder().AppendHtml (top.Name));

                Console.Write ("<div id='d");
                Console.Write (Num);
                Console.Write ("' class='s2'>");
            }

            if (showFiles && top.FileInfos.Length > 0)
            {
                var fileIndent = GetIndent(true).ToString();
                foreach (var fInfo in top.FileInfos)
                {
                    Console.Write (fileIndent);
                    Console.WriteLine (new StringBuilder().AppendHtml (fInfo.Name));
                }
                Console.WriteLine (fileIndent);
            }

            if (Depth >= 1 && ! HasSubdirs)
            {
                if (top.FileInfos != null && top.FileInfos.Length > 0)
                    Console.Write ("</div>");
                for (int dx = Depth; dx > 1 && items[dx].IsLast; --dx)
                    Console.Write ("</div>");
            }
        }
    }


    public class DirTreeScanner437 : IEnumerable<DirTreeLocation437>
    {
        private DirTreeLocation437 location;

        public DirTreeScanner437 (string dirPath, int tabSize = 4)
        { this.location = new DirTreeLocation437 (dirPath, tabSize); }

        public DirTreeScanner437 (DirectoryInfo dirInfo)
        { this.location = new DirTreeLocation437 (dirInfo); }

        IEnumerator IEnumerable.GetEnumerator()
        { return GetEnumerator(); }

        public IEnumerator<DirTreeLocation437> GetEnumerator()
        {
            while (location.Advance())
                yield return location;
        }
    }
}
