using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace FullSys
{
    public static class Win32Imports
    {
        [DllImport ("shlwapi.dll", CharSet = CharSet.Unicode)]
        public static extern int StrCmpLogicalW (string s1, string s2);
    }

    public class NaturalCompare : IComparer<DirectoryInfo>
    {
        static public readonly IComparer<DirectoryInfo> Instance = new NaturalCompare();

        public int Compare (DirectoryInfo d1, DirectoryInfo d2)
        { return Win32Imports.StrCmpLogicalW (d1.Name, d2.Name); }
    }


    [DebuggerDisplay(@"\{{Path}}")]
    public partial class DirLocation
    {
        private DirLocation (DirectoryInfo[] dirInfos, int index)
        { this.dirInfos = dirInfos; this.Index = index; }

        private DirectoryInfo[] dirInfos;
        public int Index { get; private set; }
        public FileInfo[] FileInfos { get; private set; }

        public string Path { get { return dirInfos[Index].FullName; } }
        public string Name { get { return dirInfos[Index].Name; } }
        public int DirCount { get { return dirInfos.Length; } }
        public bool IsLast { get { return Index >= DirCount-1; } }


        public class ItemStack
        {
            protected readonly IList<DirLocation> items;
            public ReadOnlyCollection<DirLocation> Items { get; private set; }

            public string RootPath { get; private set; }
            public int Depth { get; private set; }

            public DirLocation Top { get { return items[Depth]; } }
            public bool HasSubdirs { get { return Depth+1 < items.Count && items[Depth+1].dirInfos.Length > 0; } }
            public string TreeName { get { return items.Count == 1? RootPath : items[items.Count-1].Name; } }

            private ItemStack()
            {
                this.items = new List<DirLocation>();
                this.Items = new ReadOnlyCollection<DirLocation> (items);
            }

            public ItemStack (DirectoryInfo rootInfo) : this()
            {
                this.RootPath = rootInfo.FullName;
                this.items.Add (new DirLocation (new DirectoryInfo[] { rootInfo }, -1));
            }

            public ItemStack (string rootPath) : this()
            {
                this.RootPath = rootPath;
                DirectoryInfo di = new DirectoryInfo (rootPath);
                this.items.Add (new DirLocation (new DirectoryInfo[] { di }, -1));
            }

            // On exit: returns true if node has subdirectories or files (if includeFiles)
            // Any subdirectories will be prefetched.
            public bool PregetContents (bool includeFiles)
            {
                bool result;
                var top = items[items.Count-1];
                if (top.Index < 0)
                {
                    result = top.dirInfos.Length > 0;
                    top = items[items.Count-2];
                }
                else
                {
                    DirectoryInfo[] nextDirs = top.dirInfos[top.Index].GetDirectories();
                    Array.Sort (nextDirs, NaturalCompare.Instance);
                    items.Add (new DirLocation (nextDirs, -1));
                    result = nextDirs.Length > 0;
                }

                if (includeFiles)
                {
                    top.FileInfos = top.dirInfos[top.Index].GetFiles();
                    result = result || top.FileInfos.Length > 0;
                }

                return result;
            }


            public void Reset()
            {
                if (items.Count == 0)
                    items.Add (new DirLocation (new DirectoryInfo[] { new DirectoryInfo (RootPath) }, -1));
                else if (items.Count > 1)
                    items.RemoveAt (1);
                items[0].Index = -1;
                Depth = 0;
            }


            public bool Advance()
            {
                if (items.Count == 0)
                    return false;

                DirLocation top = items[items.Count-1];
                if (top.Index >= 0)
                {
                    DirectoryInfo[] subdirs = top.dirInfos[top.Index].GetDirectories();
                    if (subdirs.Length > 0)
                    {
                        Depth = items.Count;
                        items.Add (new DirLocation (subdirs, 0));
                        return true;
                    }
                }

                for (;;)
                {
                    if (++top.Index < top.DirCount)
                    { Depth = items.Count-1; return true; }
                    items.RemoveAt (items.Count-1);
                    if (items.Count == 0)
                        return false;
                    top = items[items.Count-1];
                };
            }
        }
    }


    public class DirScanner : IEnumerable<DirLocation.ItemStack>
    {
        private DirLocation.ItemStack location;

        public DirScanner (string rootPath)
        { this.location = new DirLocation.ItemStack (rootPath); }

        public DirScanner (DirectoryInfo dirInfo)
        { this.location = new DirLocation.ItemStack (dirInfo); }

        IEnumerator IEnumerable.GetEnumerator()
        { return GetEnumerator(); }

        public IEnumerator<DirLocation.ItemStack> GetEnumerator()
        {
            while (location.Advance())
                yield return location;
        }
    }
}
