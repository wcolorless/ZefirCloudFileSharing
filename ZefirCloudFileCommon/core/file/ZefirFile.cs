using System;
using System.Collections.Generic;
using System.Text;

namespace ZefirCloudFileCommon.core.file
{
    public class ZefirFile
    {
        public string FileName { get; set; }
        public string Extension { get; set; }
        public string Directory { get; set; }
        public long Size { get; set; }
        public byte[] FileData { get; set; }
        public DateTime CreateOrUpdateFile { get; set; }
    }
}
