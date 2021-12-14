using System;
using System.IO;

namespace Tests
{
    internal class LockedFile : IDisposable
    {
        private FileStream file;

        public LockedFile(string path)
        {
            file = File.OpenWrite(path);
        }

        public void Dispose()
        {
            var path = file.Name;
            file.Close();
            File.Delete(path);
        }
    }
}
