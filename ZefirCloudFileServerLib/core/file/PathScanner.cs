using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ZefirCloudFileCommon.core.file.Info;

namespace ZefirCloudFileServerLib.core.file
{
    public class PathScanner
    {
        public static ZefirDirectoryInfo Scan(string path, bool isInitial = true)
        {
            var _rootInfo = isInitial
            ? new ZefirDirectoryInfo()
            {
                IsRoot = true,
                Name = "root",
            }
            : new ZefirDirectoryInfo()
            {
                IsRoot = false,
                Name = Path.GetRelativePath("Files", path).Replace("\\", "/") 
            };
            var files = Directory.EnumerateFiles(path).ToList();
            var dirs = Directory.EnumerateDirectories(path).ToList();
            _rootInfo.Files = files.Select(x => new ZefirFileInfo
            {
                FileName = Path.GetFileName(x),
                Extension = (string)Path.GetExtension(x),
                Size = new System.IO.FileInfo(x).Length,
                Directory = Path.GetRelativePath("Files", x)
            }).ToList();
            _rootInfo.Directories = dirs.Select(x => Scan(x, false)).ToList();

            return _rootInfo;
        }
    }
}
