using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace FullSys
{
    [DebuggerDisplay(@"\{{Path}}")]
    public class DirTreeLocation : DirLocation.ItemStack
    {
        public DirTreeLocation (string rootPath, int tabSize = 4) : base (rootPath)
        { this.IndentSize = tabSize; }

        public DirTreeLocation (DirectoryInfo rootInfo, int tabSize = 4) : base (rootInfo)
        { this.IndentSize = tabSize; }

        public int Num { get; protected set; }
        public int IndentSize { get; private set; }
        public virtual string DrawUpDown { get { return "|"; } }
        public virtual string DrawLeftRight { get { return "-"; } }
        public virtual string DrawUpRight { get { return "\\"; } }
        public virtual string DrawUpDownRight { get { return "+"; } }

        public StringBuilder GetIndent (bool forFiles = false)
        {
            var result = new StringBuilder();
            var topIx = items.Count-1;
            if (! forFiles && items[topIx].Index < 0)
                --topIx;

            for (var ix = 1; ix <= topIx; ++ix)
                if (! forFiles && ix == topIx)
                {
                    if (items[ix].IsLast)
                        result.Append (DrawUpRight);
                    else
                        result.Append (DrawUpDownRight);
                    for (int kk = 1; kk < IndentSize; ++kk)
                        result.Append (DrawLeftRight);
                }
                else
                {
                    if (items[ix].IsLast)
                        result.Append (' ');
                    else
                        result.Append (DrawUpDown);
                    for (int kk = 1; kk < IndentSize; ++kk)
                        result.Append (' ');
                }

            return result;
        }

        public void WriteNodeAsText (bool showFiles)
        {
            Console.Write (GetIndent());
            Console.WriteLine (TreeName);

            if (showFiles)
            {
                PregetContents (true);
                FileInfo[] dirFiles = Top.FileInfos;
                if (dirFiles.Length > 0)
                {
                    var fileIndent = GetIndent(true).ToString();
                    foreach (var fInfo in dirFiles)
                    {
                        Console.Write (fileIndent);
                        Console.WriteLine (fInfo.Name);
                    }
                    Console.WriteLine (fileIndent);
                }
            }
        }
    }


    public class DirTreeScanner : IEnumerable<DirTreeLocation>
    {
        private DirTreeLocation location;

        public DirTreeScanner (string dirPath, int tabSize = 4)
        { this.location = new DirTreeLocation (dirPath, tabSize); }

        public DirTreeScanner (DirectoryInfo dirInfo)
        { this.location = new DirTreeLocation (dirInfo); }

        public IEnumerator<DirTreeLocation> GetEnumerator()
        {
            while (location.Advance())
                yield return location;
        }

        IEnumerator IEnumerable.GetEnumerator()
        { return GetEnumerator(); }
    }
}
