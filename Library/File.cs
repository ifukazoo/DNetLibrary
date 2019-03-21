using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Library
{
    public class Files
    {
        public delegate bool Matcher(string path);

        public static List<string> CollectFiles(string dirPath, Matcher matcher)
        {
            var collectingFiles = new ConcurrentQueue<string>();
            CollectFilesRec(dirPath, matcher, collectingFiles);
            return collectingFiles.ToList();
        }

        private static void CollectFilesRec(string dirPath, Matcher matcher, ConcurrentQueue<string> collectingFiles)
        {
            var info = new DirectoryInfo(dirPath);
            Parallel.ForEach(info.EnumerateDirectories(), (dir) => CollectFilesRec(dir.FullName, matcher, collectingFiles));
            var files = info.EnumerateFiles();
            foreach (var f in files)
            {
                if (matcher.Invoke(f.FullName))
                {
                    collectingFiles.Enqueue(f.FullName);
                }
            }
        }
    }
}
