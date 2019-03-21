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

        public static (List<string>, List<string>) CollectFiles(string dirPath, Matcher matcher)
        {
            var collectingFiles = new ConcurrentQueue<string>();
            var errFiles = new ConcurrentQueue<string>();
            CollectFilesRec(dirPath, matcher, collectingFiles, errFiles);
            return (collectingFiles.ToList(), errFiles.ToList());
        }

        private static void CollectFilesRec(string dirPath, Matcher matcher, ConcurrentQueue<string> collecting, ConcurrentQueue<string> errs)
        {
            try
            {
                var info = new DirectoryInfo(dirPath);
                var files = info.EnumerateFiles();
                foreach (var f in files)
                {
                    if (matcher.Invoke(f.FullName))
                    {
                        collecting.Enqueue(f.FullName);
                    }
                }
                Parallel.ForEach(info.EnumerateDirectories(), (dir) => CollectFilesRec(dir.FullName, matcher, collecting, errs));
            }
            catch (UnauthorizedAccessException e)
            {
                errs.Enqueue(e.Message);
            }
        }
    }
}
