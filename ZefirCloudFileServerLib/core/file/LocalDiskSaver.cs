using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ZefirCloudFileServerLib.core.file
{
    public class LocalDiskSaver
    {
        private string _SaveDirectory;

        public LocalDiskSaver(string SaveDirectory)
        {
            _SaveDirectory = SaveDirectory;
        }

        public bool SaveFile(byte[] data, string clientLogin, string filename)
        {
            var path = $"{_SaveDirectory}/{clientLogin}/{filename}";
            var fileName = Path.GetFileName(path);
            var dirPath = path.Replace(fileName, "");
            if (!ExistDirAndPermission(dirPath))
            {
                CreatePath(dirPath);
            }
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                fs.Write(data, 0, data.Length);
                fs.Close();
                return true;
            }
        }

        private void CreatePath(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        private bool ExistDirAndPermission(string path)
        {
            return Directory.Exists(path);
        }
    }
}
