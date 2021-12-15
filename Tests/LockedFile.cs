using System;
using System.IO;

namespace Tests
{
    internal class LockedFile : IDisposable
    {
        private FileStream file;
        bool DeleteAfterDispose = true;

        public LockedFile(string path, bool deleteAfterDispose = true)
        {
            file = File.OpenWrite(path);
            DeleteAfterDispose = deleteAfterDispose;
        }

        public void Dispose()
        {
            var path = file.Name;
            file.Close();

            if (DeleteAfterDispose)
                File.Delete(path);
        }
    }
}
