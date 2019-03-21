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

        public static (List<string>, List<string>) CollectPath(string dirPath, Matcher matcher)
        {
            var collecting = new ConcurrentQueue<string>();
            var errs = new ConcurrentQueue<string>();
            CollectPathRec(dirPath, matcher, collecting, errs);
            return (collecting.ToList(), errs.ToList());
        }

        private static void CollectPathRec(string dirPath, Matcher matcher, ConcurrentQueue<string> collecting, ConcurrentQueue<string> errs)
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
                Parallel.ForEach(info.EnumerateDirectories(), (dir) => CollectPathRec(dir.FullName, matcher, collecting, errs));
            }
            catch (UnauthorizedAccessException e)
            {
                errs.Enqueue(e.Message);
            }
        }
    }
}
