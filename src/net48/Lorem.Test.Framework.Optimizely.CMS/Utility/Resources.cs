using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Lorem.Test.Framework.Optimizely.CMS.Utility
{
    public abstract class Resources
    {
        private readonly List<string> _files;

        public Resources(string path)
        {
            Path = path;
            _files = new List<string>();

            Load();
        }

        public string Path { get; }

        public FileInfo GetFileInfo(string file)
        {
            string path = Path + file;

            if (!File.Exists(path))
            {
                throw new FileNotFoundException();
            }

            return new FileInfo(path);
        }

        public List<string> Get(string prefix, int max = 0)
        {
            var hits = _files.FindAll(f => f.StartsWith(prefix, System.StringComparison.CurrentCultureIgnoreCase))
                .Select(f => (Path + f).Replace('/', System.IO.Path.DirectorySeparatorChar));

            if (max > 0)
            {
                return hits.Take(max).ToList();
            }

            return hits.ToList();
        }

        public string GetRandom(string prefix)
        {
            return Get(prefix).FirstOrDefault();
        }

        private void Load()
        {
            foreach (var file in Directory.GetFiles(Path, "*.*", SearchOption.AllDirectories))
            {
                _files.Add(
                    file
                    .Replace(Path, "")
                    .Replace("\\", "/")
                );
            }
        }
    }
}
