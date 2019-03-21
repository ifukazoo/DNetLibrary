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
    public class File
    {
        public delegate bool Matcher(string path);

        public static List<string> CollectFiles(string dirPath, Matcher matcher)
        {
            var collectingFiles = new ConcurrentQueue<string>();
            var finisher = new ConcurrentDictionary<string, bool>();
            CollectFilesRec(dirPath, matcher, collectingFiles, finisher);
            while (finisher.IsEmpty || finisher.Any((e) => !e.Value))
            {
                Thread.Sleep(10);
            }
            return collectingFiles.ToList();
        }

        private static void CollectFilesRec(string dirPath, Matcher matcher, ConcurrentQueue<string> collectingFiles, ConcurrentDictionary<string, bool> finisher)
        {
            finisher[dirPath] = false;
            var info = new DirectoryInfo(dirPath);
            var dirs = info.EnumerateDirectories();
            foreach (var dir in dirs)
            {
                ThreadPool.QueueUserWorkItem((_) =>
                {
                    CollectFilesRec(dir.FullName, matcher, collectingFiles, finisher);
                });
                while (!finisher.ContainsKey(dir.FullName))
                {
                    Thread.Sleep(1);
                }
            }
            var files = info.EnumerateFiles();
            foreach (var f in files)
            {
                if (matcher.Invoke(f.FullName))
                {
                    collectingFiles.Enqueue(f.FullName);
                }
            }
            finisher[dirPath] = true;
        }
    }
}
