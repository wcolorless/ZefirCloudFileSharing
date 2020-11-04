using System;
using System.Collections.Generic;
using System.Text;

namespace ZefirCloudFileCommon.core.file.Info
{
    public class ZefirDirectoryInfo
    {
        public bool IsRoot { get; set; }
        public string ParentDir { get; set; }
        public string Name { get; set; }
        public List<ZefirFileInfo> Files { get; set; }
        public List<ZefirDirectoryInfo> Directories { get; set; }
    }
}
